import React, { useContext, useEffect, useLayoutEffect, useState } from "react";
import { StyleSheet, View, ActivityIndicator, Button } from "react-native";
import DestinationContext from "../contexts/DestinationContext";
import { BASE_URL } from "../constants";
import { Reststop } from "../models/Reststop";
import ReststopsMap from "../components/ReststopsMap";
import ResultList from "../components/ResultList";
import { useNavigation } from "@react-navigation/native";
import { useTranslation } from "react-i18next";
import ReststopContext from "../contexts/ReststopContext";
import useLocation from "../hooks/useLocation";

const Map = async () => {
	const [isLoading, setLoading] = useState(false);
	const [reststops, setReststops] = useState<Reststop[]>([]);
	const [route, setRoute] = useState<string>();
	const destinationContext = useContext(DestinationContext.Context);
	const navigation = useNavigation();
	const [t] = useTranslation();

	const userLocation = await useLocation(
		{
			enableHighAccuracy: true
		},
		[]
	);

	useLayoutEffect(() => {
		navigation.setOptions({
			headerRight: () => <Button onPress={() => {}} title={t("refresh")} />
		});
	}, [navigation]);

	useEffect(() => {
		const start = userLocation?.coords;
		const end = destinationContext.state.destination;

		console.log(start, end);

		if (!start || !end) return;

		setLoading(true);

		fetch(
			`${BASE_URL}/reststops?startLat=${start?.latitude}&startLon=${start?.longitude}&endLat=${end?.latitude}&endLon=${end?.longitude}`
		)
			.then((response) => response.json())
			.then((json) => {
				console.log(json);
				setReststops(json.reststops);
				setRoute(json.route);
			})
			.catch((error) => console.error(error))
			.finally(() => setLoading(false));
	}, [destinationContext.state.destination, userLocation]);

	return (
		<View style={{ flex: 1 }}>
			<ReststopContext.ContextProvider>
				<View style={styles.map}>
					<ReststopsMap
						route={route}
						reststops={reststops}
						userLocation={userLocation}
					></ReststopsMap>
				</View>
				<View style={styles.results}>
					{isLoading ? <ActivityIndicator /> : <ResultList reststops={reststops}></ResultList>}
				</View>
			</ReststopContext.ContextProvider>
		</View>
	);
};

const styles = StyleSheet.create({
	map: {
		flex: 1
	},
	results: {
		flex: 2
	}
});

export default Map;
