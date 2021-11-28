use std::env;

use anyhow::Result;
use geo::{Coordinate, LineString};
use polyline;
use serde::{Deserialize, Serialize};

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct RouteResult {
    pub routes: Vec<Route>,
    pub waypoints: Vec<Waypoint>,
    pub code: String,
    pub uuid: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Route {
    #[serde(rename = "weight_name")]
    pub weight_name: String,
    pub weight: f64,
    pub duration: f64,
    pub distance: f64,
    pub legs: Vec<Leg>,
    pub geometry: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Leg {
    #[serde(rename = "via_waypoints")]
    pub via_waypoints: Vec<u8>,
    pub admins: Vec<Admin>,
    pub weight: f64,
    pub duration: f64,
    pub steps: Vec<u8>,
    pub distance: f64,
    pub summary: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Admin {
    #[serde(rename = "iso_3166_1_alpha3")]
    pub iso_3166_1_alpha3: String,
    #[serde(rename = "iso_3166_1")]
    pub iso_3166_1: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Waypoint {
    pub distance: f64,
    pub name: String,
    pub location: Vec<f64>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct TableResult {
    pub code: String,
    pub durations: Vec<Vec<f64>>,
    pub destinations: Vec<Destination>,
    pub sources: Vec<Source>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Destination {
    pub distance: f64,
    pub name: String,
    pub location: Vec<f64>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Source {
    pub distance: f64,
    pub name: String,
    pub location: Vec<f64>,
}

pub struct Osrm {}

impl Osrm {
    pub async fn route(coordinates: Vec<Coordinate<f64>>) -> Result<LineString<f64>> {
        let url = Osrm::build_url(coordinates, "directions/v5");
        let route_result = reqwest::get(url).await?.json::<RouteResult>().await?;

        let polyline = polyline::decode_polyline(&route_result.routes[0].geometry, 5).unwrap();

        Ok(polyline)
    }

    pub async fn table(coordinates: Vec<Coordinate<f64>>) -> Result<Vec<Vec<f64>>> {
        let url = Osrm::build_url(coordinates, "directions-matrix/v1");
        let table_result = reqwest::get(url).await?.json::<TableResult>().await?;

        Ok(table_result.durations)
    }

    fn build_url(coordinates: Vec<Coordinate<f64>>, service: &str) -> String {
        let base_url = "https://api.mapbox.com";

        let url_coordinate_strings: Vec<String> = coordinates
            .iter()
            .map(|coord| format!("{},{}", coord.x, coord.y))
            .collect();

        let url = format!(
            "{}/{}/mapbox/driving/{}?access_token={}",
            base_url,
            service,
            url_coordinate_strings.join(";"),
            env::var("MAPBOX_API_TOKEN").unwrap()
        );

        return url;
    }
}
