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

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct TableResult {
    pub code: String,
    pub durations: Vec<Vec<f64>>,
    pub sources: Vec<Source>,
    pub destinations: Vec<Destination>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct Source {
    pub hint: String,
    pub distance: f64,
    pub location: Vec<f64>,
    pub name: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
pub struct Destination {
    pub hint: String,
    pub distance: f64,
    pub location: Vec<f64>,
    pub name: String,
}

pub struct Osrm {}

impl Osrm {
    pub async fn route(coordinates: Vec<Coordinate<f64>>) -> Result<LineString<f64>> {
        let url = Osrm::build_url(coordinates, "route");
        let route_result = reqwest::get(url).await?.json::<RouteResult>().await?;

        let polyline = polyline::decode_polyline(&route_result.routes[0].geometry, 5).unwrap();

        Ok(polyline)
    }

    pub async fn table(coordinates: Vec<Coordinate<f64>>) -> Result<Vec<Vec<f64>>> {
        let url = Osrm::build_url(coordinates, "table");
        let table_result = reqwest::get(url).await?.json::<TableResult>().await?;

        Ok(table_result.durations)
    }

    fn build_url(coordinates: Vec<Coordinate<f64>>, service: &str) -> String {
        let base_url = "https://routing.openstreetmap.de/routed-car";

        let url_coordinate_strings: Vec<String> = coordinates
            .iter()
            .map(|coord| format!("{},{}", coord.x, coord.y))
            .collect();

        let url = format!(
            "{}/{}/v1/driving/{}",
            base_url,
            service,
            url_coordinate_strings.join(";")
        );
        return url;
    }
}
