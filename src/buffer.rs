use geo::{LineString, Polygon};
use geo_clipper::{Clipper, EndType, JoinType};

pub trait Buffer {
    fn buffer(&self, delta: f64) -> Polygon<f64>;
}

impl Buffer for LineString<f64> {
    fn buffer(&self, delta: f64) -> Polygon<f64> {
        let mut points = self.to_owned().into_points();
        let mut points_reversed = points.clone();
        points_reversed.reverse();
        points.append(&mut points_reversed);

        let poly = Polygon::new(LineString::from(points), vec![]);
        let buffered_poly = poly.offset(delta, JoinType::Square, EndType::ClosedLine, 1000.);

        let polygon = buffered_poly.iter().next().unwrap().to_owned();

        return polygon;
    }
}
