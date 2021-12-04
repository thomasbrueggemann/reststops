use std::collections::HashMap;

use anyhow::Result;
use mongodb::bson::DateTime;
use serde::{Deserialize, Serialize};

use crate::reststop::{Location, Reststop, ReststopCategory};

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct OverpassResult {
    pub version: f64,
    pub generator: String,
    pub osm3s: Osm3s,
    pub elements: Vec<Element>,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Osm3s {
    #[serde(rename = "timestamp_osm_base")]
    pub timestamp_osm_base: String,
    #[serde(rename = "timestamp_areas_base")]
    pub timestamp_areas_base: Option<String>,
    pub copyright: String,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Center {
    pub lat: f64,
    pub lon: f64,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Element {
    #[serde(rename = "type")]
    pub type_field: String,
    pub id: i64,
    #[serde(default)]
    pub nodes: Vec<i64>,
    pub tags: Option<HashMap<String, String>>,
    pub lat: Option<f64>,
    pub lon: Option<f64>,
    pub center: Option<Center>,
}

pub struct Overpass {}

impl Overpass {
    pub async fn query(
        min_lat: f64,
        min_lon: f64,
        max_lat: f64,
        max_lon: f64,
    ) -> Result<Vec<Reststop>> {
        let query_bbox = format!("{},{},{},{}", min_lat, min_lon, max_lat, max_lon);
        let query = format!("[out:json][timeout:250];(way[\"highway\"=\"rest_area\"]({bbox}); node[\"highway\"=\"rest_area\"]({bbox});way[\"highway\"=\"services\"]({bbox}); node[\"highway\"=\"services\"]({bbox}););out center;", bbox = query_bbox);
        let params = [("data", query)];

        let client = reqwest::Client::new();
        let result = client
            .post("https://lz4.overpass-api.de/api/interpreter")
            .form(&params)
            .send()
            .await?
            .json::<OverpassResult>()
            .await?;

        let reststops: Vec<Reststop> = result
            .elements
            .iter()
            .filter(|element| element.tags.is_some())
            .map(|element| {
                let tags = element.tags.as_ref().unwrap();
                let category = if tags["highway"].eq("services") {
                    ReststopCategory::Services
                } else {
                    ReststopCategory::Restarea
                };

                let coords: Vec<f64> = if element.nodes.len() == 0 {
                    vec![element.lon.unwrap(), element.lat.unwrap()]
                } else {
                    let center = element.center.as_ref().unwrap();
                    vec![center.lon, center.lat]
                };

                let location = Location {
                    r#type: "Point".to_string(),
                    coordinates: coords,
                };

                Reststop {
                    id: element.id,
                    name: Overpass::get_optional_tag_value(&tags, "name"),
                    description: Overpass::get_optional_tag_value(&tags, "description"),
                    category,
                    location,
                    last_updated_utc: DateTime::now(),
                    tags: tags.to_owned(),
                }
            })
            .collect();

        Ok(reststops)
    }

    fn get_optional_tag_value(tags: &HashMap<String, String>, key: &str) -> Option<String> {
        if tags.contains_key(key) {
            Some(tags[key].to_string())
        } else {
            None
        }
    }
}
