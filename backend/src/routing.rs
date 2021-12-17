use anyhow::Result;
use geo::{Coordinate, LineString};
use polyline;
use serde::{Deserialize, Serialize};

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct RouteResult {
    pub code: String,
    pub waypoints: Vec<Waypoint>,
    pub routes: Vec<Route>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Waypoint {
    pub hint: String,
    pub distance: f64,
    pub location: Vec<f64>,
    pub name: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Route {
    pub legs: Vec<Leg>,
    #[serde(rename = "weight_name")]
    pub weight_name: String,
    pub geometry: String,
    pub weight: f64,
    pub distance: f64,
    pub duration: f64,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Leg {
    pub steps: Vec<()>,
    pub weight: f64,
    pub distance: f64,
    pub summary: String,
    pub duration: f64,
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

pub struct Routing {}

impl Routing {
    pub async fn route(coordinates: Vec<Coordinate<f64>>) -> Result<LineString<f64>> {
        let url = Routing::build_url(coordinates, "route/v1");
        let route_result = reqwest::get(url).await?.json::<RouteResult>().await?;

        let polyline = polyline::decode_polyline(&route_result.routes[0].geometry, 5).unwrap();

        Ok(polyline)
    }

    pub async fn table(coordinates: Vec<Coordinate<f64>>) -> Result<Vec<Vec<f64>>> {
        let url = Routing::build_url(coordinates, "table/v1");
        let table_result = reqwest::get(url).await?.json::<TableResult>().await?;

        Ok(table_result.durations)
    }

    fn build_url(coordinates: Vec<Coordinate<f64>>, service: &str) -> String {
        let base_url = "http://router.project-osrm.org";

        let url_coordinate_strings: Vec<String> = coordinates
            .iter()
            .map(|coord| format!("{},{}", coord.x, coord.y))
            .collect();

        let url = format!(
            "{}/{}/driving/{}",
            base_url,
            service,
            url_coordinate_strings.join(";")
        );

        return url;
    }
}
