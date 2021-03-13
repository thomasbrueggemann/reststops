import { NavigationContainer } from "@react-navigation/native";
import React from "react";
import { StatusBar } from "react-native";
import DestinationContext from "./contexts/DestinationContext";
import Destination from "./screens/Destination";
import { createStackNavigator } from "@react-navigation/stack";
import Map from "./screens/Map";
import "./i18n";

const Stack = createStackNavigator();

const App = () => {
	return (
		<DestinationContext.ContextProvider>
			<NavigationContainer>
				<StatusBar barStyle="dark-content" />
				<Stack.Navigator>
					<Stack.Screen
						name="Destination"
						component={Destination}
						options={{ title: "Set your destination" }}
					/>
					<Stack.Screen name="Map" component={Map} options={{ title: "Reststops" }} />
				</Stack.Navigator>
			</NavigationContainer>
		</DestinationContext.ContextProvider>
	);
};

export default App;
