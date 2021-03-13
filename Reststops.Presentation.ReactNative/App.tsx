import { NavigationContainer } from "@react-navigation/native";
import React from "react";
import { StatusBar } from "react-native";
import DestinationContext from "./contexts/DestinationContext";
import Destination from "./screens/Destination";
import { createStackNavigator } from "@react-navigation/stack";
import Map from "./screens/Map";
import { useTranslation } from "react-i18next";
import "./i18n";

const Stack = createStackNavigator();

const App = () => {
	const [t, i18n] = useTranslation();

	return (
		<DestinationContext.ContextProvider>
			<NavigationContainer>
				<StatusBar barStyle="dark-content" />
				<Stack.Navigator>
					<Stack.Screen
						name="Destination"
						component={Destination}
						options={{ title: t("setYourDestination") }}
					/>
					<Stack.Screen name="Map" component={Map} options={{ title: t("reststops") }} />
				</Stack.Navigator>
			</NavigationContainer>
		</DestinationContext.ContextProvider>
	);
};

export default App;
