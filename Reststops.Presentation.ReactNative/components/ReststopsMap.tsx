import React, { useContext, useEffect, useRef } from "react";
import { StyleSheet } from "react-native";
import { GeoPosition } from "react-native-geolocation-service";
import MapView, { Marker, Polyline } from "react-native-maps";
import ReststopContext from "../contexts/ReststopContext";
import { Reststop } from "../models/Reststop";

const polyline = require("@mapbox/polyline");

export interface MapViewProps {
	route?: string;
	reststops: Reststop[];
	userLocation: GeoPosition | undefined;
}

const ReststopsMap = (props: MapViewProps) => {
	const { route, reststops, userLocation } = props;
	const routeCoordinates: number[][] = route ? polyline.decode(route) : [];

	const reststopContext = useContext(ReststopContext.Context);
	const map = useRef<MapView>(null);

	useEffect(() => {
		map.current?.fitToElements(true);
	}, [reststops]);

	const selectedReststop = reststopContext.state.selected;
	useEffect(() => {
		if (userLocation && selectedReststop) {
			map.current?.fitToCoordinates(
				[
					{
						latitude: selectedReststop.latitude,
						longitude: selectedReststop.longitude
					},
					{
						latitude: userLocation.coords.latitude,
						longitude: userLocation.coords.longitude
					}
				],
				{
					edgePadding: {
						top: 10,
						right: 10,
						left: 10,
						bottom: 10
					},
					animated: true
				}
			);
		}
		map.current?.fitToCoordinates();
	}, [selectedReststop, userLocation]);

	return (
		<MapView
			ref={map}
			style={styles.map}
			showsUserLocation={true}
			followsUserLocation={true}
			showsMyLocationButton={true}
		>
			<Polyline
				coordinates={routeCoordinates.map((c) => {
					return { latitude: c[0], longitude: c[1] };
				})}
				strokeColor="rgb(45,156,219)"
				strokeWidth={4}
			/>

			{reststops.map((reststop) => {
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
