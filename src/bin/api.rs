#[macro_use]
extern crate rocket;

use ::reststops::buffer::Buffer;
use ::reststops::osrm::Osrm;
use ::reststops::reststop::{Reststop, ReststopCategory};
use futures::stream::TryStreamExt;
use geo_types::Coordinate;
use mongodb::bson::doc;
use mongodb::options::ClientOptions;
use mongodb::{Client, Collection};
use rocket::fairing::{Fairing, Info, Kind};
use rocket::http::Header;
use rocket::response::content::Json;
use rocket::{Request, Response};
use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use std::env;

#[derive(Debug, Serialize, Deserialize)]
struct ReststopDTO {
    #[serde(skip_serializing_if = "Option::is_none")]
    pub name: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,
    pub category: ReststopCategory,
    pub location: Vec<f64>,
    pub tags: HashMap<String, String>,
    pub detour_seconds: u32,
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
    max_detour_seconds: i32,
) -> Json<Vec<u8>> {
    let start = Coordinate {
        x: start_lon,
        y: start_lat,
    };

    let end = Coordinate {
        x: end_lon,
        y: end_lat,
    };

    let route = Osrm::route(vec![start, end]).await.unwrap();
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

    println!("{}", filter);

    let reststops_col = get_reststops_collection().await;
    let result: Vec<Reststop> = reststops_col
        .find(filter, None)
        .await
        .unwrap()
        .try_collect()
        .await
        .unwrap();

    println!("{} reststops found", result.len());

    // TODO: enrich with detours

    Json(vec![32])
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
