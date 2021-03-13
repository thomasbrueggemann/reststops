import React, { useContext, useEffect, useState } from "react";
import {
	ActivityIndicator,
	FlatList,
	StyleSheet,
	Text,
	TextInput,
	TouchableOpacity,
	View
} from "react-native";
import Place from "../models/Place";
import DestinationContext, { DestinationActions } from "../contexts/DestinationContext";
import { StackNavigationProp } from "@react-navigation/stack";
import { BASE_URL } from "../constants";
import Geolocation, { GeolocationResponse } from "@react-native-community/geolocation";

interface PlaceWithDistance {
	place: Place;
	distance: number;
}

const toRad = (degrees: number): number => {
	var pi = Math.PI;
	return degrees * (pi / 180);
};

const distanceInMeters = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
	var R = 6371; // km
	var dLat = toRad(lat2 - lat1);
	var dLon = toRad(lon2 - lon1);
	var lat1 = toRad(lat1);
	var lat2 = toRad(lat2);

	var a =
		Math.sin(dLat / 2) * Math.sin(dLat / 2) +
		Math.sin(dLon / 2) * Math.sin(dLon / 2) * Math.cos(lat1) * Math.cos(lat2);
	var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	var d = R * c;

	return d / 1000;
};

const sortAndFilterPlacesByDistanceToCurrentLocation = (
	places: Place[],
	currentLocation: GeolocationResponse | undefined
): Place[] => {
	if (!currentLocation) return places;

	return places
		.map(
			(place): PlaceWithDistance => {
				return {
					place: place,
					distance: distanceInMeters(
						place.latitude,
						place.longitude,
						currentLocation.coords.latitude,
						currentLocation.coords.longitude
					)
				};
			}
		)
		.sort((a, b) => a.distance - b.distance)
		.map((placeWithDistance) => placeWithDistance.place);
};

const Destination = ({ navigation }: { navigation: StackNavigationProp<any, "Destination"> }) => {
	const [isLoading, setLoading] = useState(false);
	const [places, setPlaces] = useState<Place[]>([]);
	const [typedDestination, setTypedDestination] = useState<string>();
	const destinationContext = useContext(DestinationContext.Context);
	const [userLocation, setUserLocation] = useState<GeolocationResponse>();

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
		const debounceTimer = setTimeout(() => {
			if (!typedDestination || typedDestination.length <= 2) return;

			setLoading(true);

			fetch(`${BASE_URL}/places?text=${typedDestination}`)
				.then((response) => response.json())
				.then((json) => setPlaces(json))
				.catch((error) => console.error(error))
				.finally(() => setLoading(false));
		}, 550);

		return () => clearTimeout(debounceTimer);
	}, [typedDestination]);

	const renderPlaceItem = ({ item }: { item: Place }) => {
		const onPress = () => {
			if (destinationContext.dispatch) {
				destinationContext.dispatch({
					...destinationContext.state,
					type: DestinationActions.SET,
					destination: item
				});
			}

			navigation.navigate("Map");
		};

		return (
			<TouchableOpacity onPress={onPress} style={styles.item}>
				<Text>{item.name}</Text>
			</TouchableOpacity>
		);
	};

	return (
		<View style={{ flex: 1, padding: 24 }}>
			<TextInput
				placeholder="What's your destination?"
				style={styles.placeInput}
				onChangeText={(text: string) => setTypedDestination(text)}
				value={typedDestination}
			/>
			{isLoading ? (
				<ActivityIndicator />
			) : (
				<FlatList<Place>
					data={sortAndFilterPlacesByDistanceToCurrentLocation(places, userLocation)}
					renderItem={renderPlaceItem}
					keyExtractor={(item) => item.name}
				/>
			)}
		</View>
	);
};

const styles = StyleSheet.create({
	item: {
		backgroundColor: "#ddd",
		borderRadius: 10,
		padding: 10,
		marginVertical: 8
	},
	placeInput: {
		height: 40,
		borderColor: "gray",
		borderWidth: 1,
		backgroundColor: "white",
		paddingLeft: 10,
		marginBottom: 10
	}
});

export default Destination;
