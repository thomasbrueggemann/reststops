import { NavigationContainer } from "@react-navigation/native";
import React, { useEffect } from "react";
import { NativeModules, Platform, StatusBar } from "react-native";
import DestinationContext from "./contexts/DestinationContext";
import Destination from "./screens/Destination";
import { createStackNavigator } from "@react-navigation/stack";
import Map from "./screens/Map";
import { useTranslation } from "react-i18next";
import "./i18n";

const Stack = createStackNavigator();

const deviceLanguage =
	Platform.OS === "ios"
		? NativeModules.SettingsManager.settings.AppleLocale ||
		  NativeModules.SettingsManager.settings.AppleLanguages[0] // iOS 13
		: NativeModules.I18nManager.localeIdentifier;

const App = () => {
	const [t, i18n] = useTranslation();

	useEffect(() => {
		const locale: string = deviceLanguage.split("_")[0].split("-")[0];
		console.log("device language", deviceLanguage, "set locale to", locale);

		i18n.changeLanguage(locale);
	}, [deviceLanguage]);

	return (
		<DestinationContext.ContextProvider>
			<NavigationContainer>
				<StatusBar barStyle="dark-content" />
				<Stack.Navigator>
					<Stack.Screen
						name="Destination"
						component={Destination}
						options={{ title: t("destination") }}
					/>
					<Stack.Screen name="Map" component={Map} options={{ title: t("reststops") }} />
				</Stack.Navigator>
			</NavigationContainer>
		</DestinationContext.ContextProvider>
	);
};

export default App;
