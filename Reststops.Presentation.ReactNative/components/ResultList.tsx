import React from "react";
import { Reststop } from "../models/Reststop";
import Timeline, { Data } from "react-native-timeline-flatlist";

export interface ResultListProps {
	reststops: Reststop[];
}

const getDescription = (reststop: Reststop): string => {
	const amenetyOptions = [
		{
			condition: hasTag(reststop, "highway", "rest_area"),
			amenety: "Rest area"
		},
		{
			condition: hasTag(reststop, "highway", "services"),
			amenety: "Rest & service area"
		},
		{
			condition: hasTag(reststop, "toilets", "yes"),
			amenety: "Toilets"
		},
		{
			condition: hasTag(reststop, "toilets", "no"),
			amenety: "No toilets"
		},
		{
			condition: hasTag(reststop, "highway", "services") || hasTag(reststop, "amenity", "fuel"),
			amenety: "Gas station"
		},
		{
			condition: hasTag(reststop, "baby_change", "yes"),
			amenety: "Baby changing room"
		},
		{
			condition: hasTag(reststop, "amenity", "parking"),
			amenety: "Parking"
		},
		{
			condition: hasTag(reststop, "tourism", "hotel"),
			amenety: "Hotel"
		},
		{
			condition: hasTag(reststop, "caravan", "yes"),
			amenety: "Caravan parking"
		},
		{
			condition: hasTag(reststop, "charging_station", "yes"),
			amenety: "Charging station"
		},
		{
			condition:
				hasTag(reststop, "fast_food", "yes") ||
				hasTag(reststop, "restaurant", "yes") ||
				hasTag(reststop, "amenity", "restaurant"),
			amenety: "Restaurant"
		},
		{
			condition: hasTag(reststop, "shower", "yes"),
			amenety: "Shower"
		},
		{
			condition: hasTag(reststop, "wheelchair", "yes"),
			amenety: "Wheelchair access"
		},
		{
			condition: hasTag(reststop, "wheelchair", "no"),
			amenety: "No wheelchair access"
		},
		{
			condition: hasTag(reststop, "waste_basket", "yes"),
			amenety: "Waste bin"
		},
		{
			condition: hasTag(reststop, "shop", "yes") || hasTag(reststop, "shop", "kiosk"),
			amenety: "Shop"
		},
		{
			condition: hasTag(reststop, "tourism", "information"),
			amenety: "Tourist information"
		},
		{
			condition: hasTag(reststop, "drinking_water", "yes"),
			amenety: "Drinking water"
		},
		{
			condition: hasTag(reststop, "drinking_water", "no"),
			amenety: "No drinking water"
		}
	];

	let desc = amenetyOptions
		.filter((option) => option.condition === true)
		.map((option) => option.amenety)
		.join(", ");

	if ("opening_hours" in reststop.tags) {
		desc += reststop.tags["opening_hours"];
	}

	return desc;
};

const getDistanceAway = (reststop: Reststop): string => {
	const km = reststop.distanceInMeters / 1000;

	if (km >= 10) return km.toFixed(0) + " km";

	return km.toFixed(1) + " km";
};

const hasTag = (reststop: Reststop, tag: string, value: string) => {
	return tag in reststop.tags && reststop.tags[tag] === value;
};

const ResultList = (props: ResultListProps) => {
	const timelineData: Data[] = props.reststops.map(
		(reststop): Data => {
			return {
				time:
					getDistanceAway(reststop) +
					`\n+${(reststop.detourDurationInSeconds / 60).toFixed(0)} min`,
				title: reststop.name || "Reststop",
				description: getDescription(reststop)
			};
		}
	);

	return (
		<Timeline
			data={timelineData}
			innerCircle="dot"
			separator={false}
			circleSize={20}
			circleColor="rgb(45,156,219)"
			lineColor="rgb(45,156,219)"
			timeContainerStyle={{ minWidth: 80, marginTop: -5 }}
			timeStyle={{
				textAlign: "center",
				backgroundColor: "#ff9797",
				color: "white",
				padding: 6,
				borderRadius: 12,
				fontWeight: "bold"
			}}
			titleStyle={{
				marginTop: -10
			}}
			detailContainerStyle={{
				marginBottom: 30
			}}
			descriptionStyle={{ color: "gray" }}
			options={{
				style: {
					paddingBottom: 25,
					paddingLeft: 20,
					paddingRight: 20,
					paddingTop: 25
				}
			}}
		/>
	);
};

export default ResultList;
