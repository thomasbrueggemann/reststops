import React from "react";
import { Reststop } from "../models/Reststop";
import Timeline, { Data } from "react-native-timeline-flatlist";

export interface ResultListProps {
	reststops: Reststop[];
}

const getDescription = (reststop: Reststop): string => {
	return "Test";
};

const getDistanceAway = (reststop: Reststop): string => {
	const km = reststop.distanceInMeters / 1000;

	if (km >= 100) return km.toFixed(0) + " km";

	return km.toFixed(1) + " km";
};

const ResultList = (props: ResultListProps) => {
	const timelineData: Data[] = props.reststops.map(
		(reststop): Data => {
			return {
				time: getDistanceAway(reststop),
				title: reststop.name,
				description: getDescription(reststop)
			};
		}
	);

	return (
		<Timeline
			data={timelineData}
			innerCircle={"dot"}
			circleSize={20}
			circleColor="rgb(45,156,219)"
			lineColor="rgb(45,156,219)"
			timeContainerStyle={{ minWidth: 52, marginTop: -5 }}
			timeStyle={{
				textAlign: "center",
				backgroundColor: "#ff9797",
				color: "white",
				padding: 5,
				borderRadius: 13
			}}
			descriptionStyle={{ color: "gray" }}
			options={{
				style: { paddingTop: 5 }
			}}
		/>
	);
};

export default ResultList;
