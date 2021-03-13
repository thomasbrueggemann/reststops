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

const Destination = ({ navigation }: { navigation: StackNavigationProp<any, "Destination"> }) => {
	const [isLoading, setLoading] = useState(false);
	const [data, setData] = useState<Place[]>([]);
	const [typedDestination, setTypedDestination] = useState<string>();
	const destinationContext = useContext(DestinationContext.Context);

	useEffect(() => {
		if (!typedDestination) return;

		setLoading(true);

		fetch(`${BASE_URL}/places?text=${typedDestination}`)
			.then((response) => response.json())
			.then((json) => setData(json))
			.catch((error) => console.error(error))
			.finally(() => setLoading(false));
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
					data={data}
					renderItem={renderPlaceItem}
					keyExtractor={(item) => item.name}
				/>
			)}
		</View>
	);
};

const styles = StyleSheet.create({
	item: {
		backgroundColor: "#f9c2ff",
		padding: 10,
		marginVertical: 8
	},
	placeInput: {
		height: 40,
		borderColor: "gray",
		borderWidth: 1
	}
});

export default Destination;
