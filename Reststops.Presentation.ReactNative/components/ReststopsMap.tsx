import React, { useEffect, useRef } from "react";
import { StyleSheet } from "react-native";
import MapView, { Marker, Polyline } from "react-native-maps";
import { Reststop } from "../models/Reststop";

const polyline = require("@mapbox/polyline");

export interface MapViewProps {
	route?: string;
	reststops: Reststop[];
}

const ReststopsMap = (props: MapViewProps) => {
	const rawRouteCoordinates: number[][] = props.route ? polyline.decode(props.route) : [];

	const map = useRef<MapView>(null);

	useEffect(() => {
		map.current?.fitToElements(true);
	}, [props]);

	return (
		<MapView
			ref={map}
			style={styles.map}
			showsUserLocation={true}
			followsUserLocation={true}
			showsMyLocationButton={true}
		>
			<Polyline
				coordinates={rawRouteCoordinates.map((c) => {
					return { latitude: c[0], longitude: c[1] };
				})}
				strokeColor="#FF0000"
				strokeWidth={4}
			/>

			{props.reststops.map((reststop) => {
				return (
					<Marker
						key={reststop.id}
						title={reststop.name ?? ""}
						coordinate={{ latitude: reststop.latitude, longitude: reststop.longitude }}
					></Marker>
				);
			})}
		</MapView>
	);
};

const styles = StyleSheet.create({
	map: {
		flex: 1
	}
});

export default ReststopsMap;
