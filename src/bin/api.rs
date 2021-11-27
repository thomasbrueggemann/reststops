#[macro_use]
extern crate rocket;

use ::reststops::buffer::Buffer;
use ::reststops::osrm::Osrm;
use ::reststops::reststop::{Reststop, ReststopCategory};
use futures::future::join_all;
use futures::stream::TryStreamExt;
use geo_types::{Coordinate, LineString};
use mongodb::bson::doc;
use mongodb::options::{ClientOptions, FindOptions};
use mongodb::{Client, Collection};
use rocket::fairing::{Fairing, Info, Kind};
use rocket::http::Header;
use rocket::serde::json::Json;
use rocket::serde::Serialize;
use rocket::{Request, Response};
use std::collections::HashMap;
use std::env;

#[derive(Serialize)]
#[serde(crate = "rocket::serde")]
struct ReststopResponse {
    #[serde(skip_serializing_if = "Option::is_none")]
    pub name: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,
    pub category: ReststopCategory,
    pub location: Vec<f64>,
    pub tags: HashMap<String, String>,
    pub detour_seconds: f64,
}

impl ReststopResponse {
    pub fn from(reststop: &Reststop, detour_seconds: f64) -> ReststopResponse {
        ReststopResponse {
            name: reststop.name.to_owned(),
            description: reststop.description.to_owned(),
            category: reststop.category,
            location: reststop.location.coordinates.to_owned(),
            tags: reststop.tags.to_owned(),
            detour_seconds,
        }
    }
}

#[get("/")]
fn index() -> &'static str {
    "Reststops v1.0.0"
}

#[get("/reststops?<start_lon>&<start_lat>&<end_lon>&<end_lat>&<max_detour_seconds>")]
async fn reststops(
    start_lon: f64,
    start_lat: f64,
    end_lon: f64,
    end_lat: f64,
    max_detour_seconds: f64,
) -> Json<Vec<ReststopResponse>> {
    let start = Coordinate {
        x: start_lon,
        y: start_lat,
    };

    let end = Coordinate {
        x: end_lon,
        y: end_lat,
    };

    let route = Osrm::route(vec![start, end]).await.unwrap();
    let reststops = get_reststops(route).await;

    let durations = get_duration_table(start, end, &reststops).await;
    let responses = assign_durations_to_reststops(&durations, &reststops);

    println!("{} reststops found", reststops.len());

    Json(responses)
}

fn assign_durations_to_reststops(
    durations: &Vec<Vec<f64>>,
    reststops: &Vec<Reststop>,
) -> Vec<ReststopResponse> {
    let total_duration = durations[0][reststops.len() - 1];

    let mut response_reststops: Vec<ReststopResponse> = vec![];

    for i in 0..reststops.len() - 1 {
        let duration_to_reststop = durations[0][i + 1];
        let duration_from_reststop = durations[i + 1][reststops.len() - 1];

        let detour_seconds = (duration_to_reststop + duration_from_reststop) - total_duration;
        let response_reststop = ReststopResponse::from(&reststops[i], detour_seconds);

        response_reststops.push(response_reststop);
    }

    return response_reststops;
}

async fn get_duration_table(
    start: Coordinate<f64>,
    end: Coordinate<f64>,
    reststops: &Vec<Reststop>,
) -> Vec<Vec<f64>> {
    let mut distance_coords: Vec<Coordinate<f64>> = vec![start];
    let mut reststop_coords: Vec<Coordinate<f64>> = reststops
        .iter()
        .map(|reststop| reststop.to_coordinate())
        .collect();

    distance_coords.append(&mut reststop_coords);
    distance_coords.push(end);

    let table = Osrm::table(distance_coords).await.unwrap();

    return table;
}

async fn get_reststops(route: LineString<f64>) -> Vec<Reststop> {
    let buffered_route = route.buffer(4.0, 10.0);

    let buffered_coordinates: Vec<Vec<f64>> = buffered_route
        .exterior()
        .points_iter()
        .map(|point| vec![point.x(), point.y()])
        .collect();

    let filter = doc! {
        "location": {
            "$geoWithin": {
                "$geometry": {
                    "type": "Polygon",
                    "coordinates": vec![buffered_coordinates]
                }
            }
        }
    };

    let find_options = FindOptions::builder().limit(100).build();

    let reststops_col = get_reststops_collection().await;
    let result: Vec<Reststop> = reststops_col
        .find(filter, find_options)
        .await
        .unwrap()
        .try_collect()
        .await
        .unwrap();

    return result;
}

async fn get_reststops_collection() -> Collection<Reststop> {
    let connection_string = env::var("MONGO_CONNECTION_STRING")
        .unwrap_or("mongodb://test:test@localhost:25015".to_string());

    let opts = ClientOptions::parse(connection_string).await.unwrap();
    let client = Client::with_options(opts).unwrap();

    let db = client.database("reststops");
    let reststops_col = db.collection::<Reststop>("reststops");
    return reststops_col;
}

pub struct CORS;

#[rocket::async_trait]
impl Fairing for CORS {
    fn info(&self) -> Info {
        Info {
            name: "Add CORS headers to responses",
            kind: Kind::Response,
        }
    }

    async fn on_response<'r>(&self, _req: &'r Request<'_>, res: &mut Response<'r>) {
        res.set_header(Header::new("Access-Control-Allow-Origin", "*"));
        res.set_header(Header::new(
            "Access-Control-Allow-Methods",
            "POST, GET, PATCH, OPTIONS",
        ));
        res.set_header(Header::new("Access-Control-Allow-Headers", "*"));
        res.set_header(Header::new("Access-Control-Allow-Credentials", "true"));
    }
}

#[launch]
fn rocket() -> _ {
    rocket::build()
        .attach(CORS)
        .mount("/", routes![index, reststops])
}
