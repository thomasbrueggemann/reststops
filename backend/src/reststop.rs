use std::collections::HashMap;

use geo::{point, Coordinate, Point};
use mongodb::bson::DateTime;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize, Copy, Clone)]
pub enum ReststopCategory {
    #[serde(rename = "restarea")]
    Restarea,
    #[serde(rename = "services")]
    Services,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct Location {
    #[serde(rename(serialize = "type"))]
    pub r#type: String,
    pub coordinates: Vec<f64>,
}

pub struct Reststop {
    pub id: i64,
    pub name: Option<String>,
    pub description: Option<String>,
    pub category: ReststopCategory,
    pub location: Location,
    pub last_updated_utc: DateTime,
    pub tags: HashMap<String, String>,
}

impl Reststop {
    pub fn to_coordinate(&self) -> Coordinate<f64> {
        Coordinate {
            x: self.location.coordinates[0],
            y: self.location.coordinates[1],
        }
    }

    pub fn to_point(&self) -> Point<f64> {
        point!(x: self.location.coordinates[0], y: self.location.coordinates[1])
    }
}
