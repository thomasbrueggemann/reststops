#[macro_use]
extern crate rocket;

use rocket::fairing::{Fairing, Info, Kind};
use rocket::http::Header;
use rocket::response::content::Json;
use rocket::{Request, Response};

#[get("/")]
fn index() -> &'static str {
    "Reststops v1.0.0"
}

#[get("/reststops?<startLon>&<startLat>&<endLon>&<endLat>&<maxDetourSeconds>")]
async fn reststops(
    startLon: f64,
    startLat: f64,
    endLon: f64,
    endLat: f64,
    maxDetourSeconds: i32,
) -> Json<Vec<u8>> {
    // TODO: calculate osrm route

    // TODO: inflate to buffered polygon

    // TODO: read restatops within polygon

    // TODO: enrich with detours

    Json(vec![32])
}

pub struct CORS;

#[rocket::async_trait]
impl Fairing for CORS {
    fn info(&self) -> Info {
        Info {
            name: "Add CORS headers to responses",
            kind: Kind::Response,
        }
    }

    async fn on_response<'r>(&self, _req: &'r Request<'_>, res: &mut Response<'r>) {
        res.set_header(Header::new("Access-Control-Allow-Origin", "*"));
        res.set_header(Header::new(
            "Access-Control-Allow-Methods",
            "POST, GET, PATCH, OPTIONS",
        ));
        res.set_header(Header::new("Access-Control-Allow-Headers", "*"));
        res.set_header(Header::new("Access-Control-Allow-Credentials", "true"));
    }
}

#[launch]
fn rocket() -> _ {
    rocket::build()
        .attach(CORS)
        .mount("/", routes![index, reststops])
}
