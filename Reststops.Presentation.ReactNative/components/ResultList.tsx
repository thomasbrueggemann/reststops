import React from "react";
import { Reststop } from "../models/Reststop";
import Timeline, { Data } from "react-native-timeline-flatlist";
import { TFunction, useTranslation } from "react-i18next";

export interface ResultListProps {
	reststops: Reststop[];
}

const getDescription = (reststop: Reststop, t: TFunction<string>): string => {
	const amenetyOptions = [
		{
			condition: hasTag(reststop, "highway", "rest_area"),
			amenety: t("amenities.restArea")
		},
		{
			condition: hasTag(reststop, "highway", "services"),
			amenety: t("amenities.servicesArea")
		},
		{
			condition: hasTag(reststop, "toilets", "yes"),
			amenety: t("amenities.toilets")
		},
		{
			condition: hasTag(reststop, "toilets", "no"),
			amenety: t("amenities.noToilets")
		},
		{
			condition: hasTag(reststop, "baby_change", "yes"),
			amenety: t("amenities.babyChange")
		},
		{
			condition: hasTag(reststop, "amenity", "parking"),
			amenety: t("amenities.parking")
		},
		{
			condition: hasTag(reststop, "tourism", "hotel"),
			amenety: t("amenities.hotel")
		},
		{
			condition: hasTag(reststop, "caravan", "yes"),
			amenety: t("amenities.caravan")
		},
		{
			condition: hasTag(reststop, "charging_station", "yes"),
			amenety: t("amenities.charginStation")
		},
		{
			condition:
				hasTag(reststop, "fast_food", "yes") ||
				hasTag(reststop, "restaurant", "yes") ||
				hasTag(reststop, "amenity", "restaurant"),
			amenety: t("amenities.restaurant")
		},
		{
			condition: hasTag(reststop, "shower", "yes"),
			amenety: t("amenities.shower")
		},
		{
			condition: hasTag(reststop, "wheelchair", "yes"),
			amenety: t("amenities.wheelchairAccess")
		},
		{
			condition: hasTag(reststop, "wheelchair", "no"),
			amenety: t("amenities.noWheelchairAccess")
		},
		{
			condition: hasTag(reststop, "waste_basket", "yes"),
			amenety: t("amenities.wasteBasket")
		},
		{
			condition: hasTag(reststop, "shop", "yes") || hasTag(reststop, "shop", "kiosk"),
			amenety: t("amenities.shop")
		},
		{
			condition: hasTag(reststop, "tourism", "information"),
			amenety: t("amenities.touristInformation")
		},
		{
			condition: hasTag(reststop, "drinking_water", "yes"),
			amenety: t("amenities.drinkingWater")
		},
		{
			condition: hasTag(reststop, "drinking_water", "no"),
			amenety: t("amenities.noDrinkingWater")
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
	const [t] = useTranslation();

	const timelineData: Data[] = props.reststops.map(
		(reststop): Data => {
			return {
				time:
					getDistanceAway(reststop) +
					`\n+${(reststop.detourDurationInSeconds / 60).toFixed(0)} min`,
				title: reststop.name || t("reststop"),
				description: getDescription(reststop, t)
			};
		}
	);

	return (
		<Timeline
			data={timelineData}
			onEventPress={(event) => {
				console.log(event);
			}}
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
