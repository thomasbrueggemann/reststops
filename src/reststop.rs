use std::collections::HashMap;

use mongodb::bson::DateTime;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct Reststop {
    #[serde(rename(serialize = "_id"))]
    pub id: i64,
    pub name: Option<String>,
    pub description: Option<String>,
    pub latitude: f64,
    pub longitude: f64,
    pub last_updated_utc: DateTime,
    pub tags: HashMap<String, String>,
}
