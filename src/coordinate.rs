pub struct Coordinate {
    pub longitude: f64,
    pub latitude: f64,
}

impl Coordinate {
    pub fn to_string(&self) -> String {
        format!("{},{}", self.longitude, self.latitude)
    }
}
