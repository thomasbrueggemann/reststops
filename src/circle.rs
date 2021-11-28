use geo::{prelude::HaversineDestination, Coordinate, LineString, Point, Polygon};

pub trait Circle {
    fn circle(center: Coordinate<f64>, radius_meters: i32, resolution_bearing: i32)
        -> Polygon<f64>;
}

impl Circle for Polygon<f64> {
    fn circle(
        center: Coordinate<f64>,
        radius_meters: i32,
        resolution_bearing: i32,
    ) -> Polygon<f64> {
        let mut ring_polygon_coords: Vec<Coordinate<f64>> = vec![];

        for i in 1..(360 / resolution_bearing) {
            let bearing = i * resolution_bearing;

            let next_point = HaversineDestination::haversine_destination(
                &Point::from(center),
                bearing as f64,
                radius_meters as f64,
            );
            ring_polygon_coords.push(Coordinate::from(next_point));
        }

        let line_string_ring = LineString(ring_polygon_coords);

        Polygon::new(line_string_ring, vec![])
    }
}
