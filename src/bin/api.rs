#[macro_use]
extern crate rocket;

use ::reststops::buffer::Buffer;
use ::reststops::circle::Circle;
use ::reststops::osrm::Osrm;
use ::reststops::overpass::Overpass;
use ::reststops::reststop::{Reststop, ReststopCategory};
use geo::algorithm::bounding_rect::BoundingRect;
use geo::prelude::{Contains, HaversineDistance};
use geo::{Coordinate, LineString, Point, Polygon};
use geo_clipper::Clipper;
use rocket::fairing::{Fairing, Info, Kind};
use rocket::http::Header;
use rocket::serde::json::Json;
use rocket::serde::Serialize;
use rocket::{Request, Response};
use std::collections::HashMap;

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
    let mut reststops = get_reststops(route, start).await;

    let closest_reststops = filter_closest_reststops(&mut reststops, Point::from(start));

    if closest_reststops.len() == 0 {
        return Json(vec![]);
    }

    let durations = get_duration_table(start, end, &closest_reststops).await;
    let responses = assign_durations_to_reststops(&durations, &closest_reststops)
        .into_iter()
        .filter(|response| response.detour_seconds <= max_detour_seconds)
        .collect::<Vec<ReststopResponse>>();

    Json(responses)
}

fn filter_closest_reststops(reststops: &mut Vec<Reststop>, start: Point<f64>) -> Vec<&Reststop> {
    reststops.sort_by(|a, b| {
        let distance_a = start.haversine_distance(&a.to_point());
        let distance_b = start.haversine_distance(&b.to_point());

        distance_a.partial_cmp(&distance_b).unwrap()
    });

    reststops.iter().take(23).collect()
}

fn assign_durations_to_reststops(
    durations: &Vec<Vec<f64>>,
    reststops: &Vec<&Reststop>,
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

    response_reststops.sort_by(|a, b| a.detour_seconds.partial_cmp(&b.detour_seconds).unwrap());

    return response_reststops;
}

async fn get_duration_table(
    start: Coordinate<f64>,
    end: Coordinate<f64>,
    reststops: &Vec<&Reststop>,
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

async fn get_reststops(route: LineString<f64>, start: Coordinate<f64>) -> Vec<Reststop> {
    let buffered_route = route.buffer(0.03);
    let circle_around_start = Polygon::circle(start, 50_000, 5);

    let sectors = buffered_route.intersection(&circle_around_start, 1000.);
    let sector: Polygon<f64> = sectors.into_iter().next().unwrap();

    let sector_bbox = sector.bounding_rect().unwrap();

    let overpass_reststops = Overpass::query(
        sector_bbox.min().x,
        sector_bbox.max().y,
        sector_bbox.max().x,
        sector_bbox.min().y,
    )
    .await
    .unwrap();

    let result: Vec<Reststop> = overpass_reststops
        .into_iter()
        .filter(|reststop| sector.contains(&reststop.to_coordinate()))
        .collect();

    return result;
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
