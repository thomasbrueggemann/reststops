import React, { useContext, useEffect, useState } from "react";
import { StyleSheet, View, Text, ScrollView, ActivityIndicator } from "react-native";
import DestinationContext from "../contexts/DestinationContext";
import { BASE_URL } from "../constants";
import { Reststop } from "../models/Reststop";
import ReststopsMap from "../components/ReststopsMap";
import Geolocation, { GeolocationResponse } from "@react-native-community/geolocation";
import ResultList from "../components/ResultList";

const Map = () => {
	const [isLoading, setLoading] = useState(false);
	const [reststops, setReststops] = useState<Reststop[]>([]);
	const [route, setRoute] = useState<string>();
	const [userLocation, setUserLocation] = useState<GeolocationResponse>();

	const destinationContext = useContext(DestinationContext.Context);

	useEffect(() => {
		Geolocation.getCurrentPosition(
			(info) => setUserLocation(info),
			() => {},
			{
				enableHighAccuracy: true,
				maximumAge: 0
			}
		);
	}, []);

	useEffect(() => {
		const start = userLocation?.coords;
		const end = destinationContext.state.destination;

		if (!start || !end) return;

		setLoading(true);

		fetch(
			`${BASE_URL}/reststops?startLat=${start?.latitude}&startLon=${start?.longitude}&endLat=${end?.latitude}&endLon=${end?.longitude}`
		)
			.then((response) => response.json())
			.then((json) => {
				setReststops(json.reststops);
				setRoute(json.route);
			})
			.catch((error) => console.error(error))
			.finally(() => setLoading(false));
	}, [destinationContext.state.destination, userLocation]);

	return (
		<View style={{ flex: 1 }}>
			<View style={styles.map}>
				<ReststopsMap route={route} reststops={reststops}></ReststopsMap>
			</View>
			<View style={styles.results}>
				{isLoading ? <ActivityIndicator /> : <ResultList reststops={reststops}></ResultList>}
			</View>
		</View>
	);
};

const styles = StyleSheet.create({
	map: {
		flex: 1
	},
	results: {
		flex: 2,
		padding: 20
	}
});

export default Map;
