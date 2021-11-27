use std::collections::HashMap;

use mongodb::bson::DateTime;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
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

#[derive(Debug, Serialize, Deserialize)]
pub struct Reststop {
    #[serde(rename(serialize = "_id"))]
    pub id: i64,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub name: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub description: Option<String>,
    pub category: ReststopCategory,
    pub location: Location,
    pub last_updated_utc: DateTime,
    pub tags: HashMap<String, String>,
}
