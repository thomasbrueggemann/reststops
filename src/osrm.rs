use anyhow::Result;
use geo_types::{Coordinate, LineString};
use polyline;
use serde::{Deserialize, Serialize};

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct RouteResult {
    pub code: String,
    pub waypoints: Vec<Waypoint>,
    pub routes: Vec<Route>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct Waypoint {
    pub hint: String,
    pub distance: f64,
    pub location: Vec<f64>,
    pub name: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct Route {
    pub legs: Vec<Leg>,
    pub weight_name: String,
    pub geometry: String,
    pub weight: f64,
    pub distance: f64,
    pub duration: f64,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct Leg {
    pub steps: Vec<i32>,
    pub weight: f64,
    pub distance: f64,
    pub summary: String,
    pub duration: f64,
}

pub struct Osrm {}

impl Osrm {
    pub async fn route(coordinates: Vec<Coordinate<f64>>) -> Result<LineString<f64>> {
        let base_url = "https://routing.openstreetmap.de/routed-car/route/v1/driving/";

        let url_coordinate_strings: Vec<String> = coordinates
            .iter()
            .map(|coord| format!("{},{}", coord.x, coord.y))
            .collect();

        let url = format!("{}{}", base_url, url_coordinate_strings.join(";"));
        let route_result = reqwest::get(url).await?.json::<RouteResult>().await?;
        let polyline = polyline::decode_polyline(&route_result.routes[0].geometry, 5).unwrap();

        Ok(polyline)
    }
}
