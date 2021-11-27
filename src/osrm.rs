use crate::coordinate::Coordinate;
use anyhow::Result;

pub struct Osrm {}

impl Osrm {
    pub async fn route(coordinates: Vec<Coordinate>) -> Result<()> {
        let mut url = "https://routing.openstreetmap.de/routed-car/route/v1/driving/".to_string();

        // 9.305419921874998,50.15578588538455;9.6624755859375,50.877044231111014?overview=false&alternatives=true&steps=true

        let body = reqwest::get("https://www.rust-lang.org")
            .await?
            .text()
            .await?;

        Ok(())
    }
}
