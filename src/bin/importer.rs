use anyhow::Result;
use mongodb::bson::{doc, DateTime};
use mongodb::options::ReplaceOptions;
use mongodb::{options::ClientOptions, Client};
use osmpbfreader::Tags;
use reststops::reststop::{Location, Reststop, ReststopCategory};
use std::collections::HashMap;
use std::env;
use std::fs::File;
use std::time::Instant;

#[tokio::main]
pub async fn main() -> Result<()> {
    let mut now = Instant::now();

    let connection_string = env::var("MONGO_CONNECTION_STRING").unwrap();
    let opts = ClientOptions::parse(connection_string).await?;
    let client = Client::with_options(opts)?;

    let args: Vec<String> = env::args().collect();
    let filepath = &args[1];

    let r = File::open(&std::path::Path::new(filepath)).unwrap();
    let mut pbf = osmpbfreader::OsmPbfReader::new(r);

    let objs = pbf
        .get_objs_and_deps(|obj| {
            obj.is_node()
                && obj.tags().contains_key("highway")
                && (obj.tags()["highway"] == "rest_area" || obj.tags()["highway"] == "services")
        })
        .unwrap();

    let db = client.database("reststops");
    let reststops_col = db.collection::<Reststop>("reststops");

    let reststops: Vec<Reststop> = objs
        .into_iter()
        .map(|(id, obj)| {
            let node = obj.node().unwrap();

            let tags: HashMap<String, String> = node
                .tags
                .iter()
                .map(|(key, value)| (key.to_string(), value.to_string()))
                .filter(|(key, _value)| {
                    !key.eq("name") && !key.eq("description") && !key.eq("highway")
                })
                .collect();

            let description = get_optional_tag_value(&node.tags, "description");
            let name = get_optional_tag_value(&node.tags, "name");

            let category = if node.tags.contains("highway", "services") {
                ReststopCategory::Services
            } else {
                ReststopCategory::Restarea
            };

            let location = Location {
                r#type: "Point".to_string(),
                coordinates: vec![node.lon(), node.lat()],
            };

            Reststop {
                id: id.inner_id(),
                name,
                category,
                description,
                location,
                last_updated_utc: DateTime::now(),
                tags,
            }
        })
        .collect();

    println!("Parsed OSM PBF file:\t\t\t{} secs", now.elapsed().as_secs());
    now = Instant::now();

    for reststop in &reststops {
        let replace_options = ReplaceOptions::builder().upsert(true).build();

        reststops_col
            .replace_one(doc! { "_id": reststop.id }, reststop, replace_options)
            .await?;
    }

    println!(
        "Upserted {} reststops to MongoDB:\t{} secs",
        reststops.len(),
        now.elapsed().as_secs()
    );

    Ok(())
}

fn get_optional_tag_value(tags: &Tags, key: &str) -> Option<String> {
    if tags.contains_key(key) {
        Some(tags[key].to_string())
    } else {
        None
    }
}
