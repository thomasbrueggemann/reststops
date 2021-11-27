pub struct Coordinate {
    longitude: f64,
    latitude: f64,
}

impl Coordinate {
    pub fn to_string(&self) -> String {
        format!("{},{}", self.longitude, self.latitude)
    }
}
