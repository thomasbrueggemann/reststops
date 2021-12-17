import { IonItem, IonLabel, IonList } from "@ionic/react";
import React, { useEffect, useState } from "react";
import { MAPBOX_TOKEN } from "../constants";
import { Feature, GeocodingResult, Geometry } from "../models/GeocodingResult";

export interface AutocompleteProps {
  destination: string;
  onDestinationSelected: (destination: Geometry) => void;
}

const Autocomplete: React.FC<AutocompleteProps> = (props) => {
  const [features, setFeatures] = useState<Feature[]>([]);

  const getGeocodingResults = async (searchText: string) => {
    if (searchText && searchText.length < 3) {
      return;
    }

    const baseUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places";
    const response = await fetch(
      `${baseUrl}/${encodeURIComponent(
        searchText
      )}.json?autocomplete=true&fuzzyMatch=true&access_token=${MAPBOX_TOKEN}`
    );

    const result: GeocodingResult = await response.json();
    setFeatures(result.features);
  };

  useEffect(() => {
    getGeocodingResults(props.destination);
  }, [props.destination]);

  return (
    <IonList>
      {features.map((feature, i) => {
        return (
          <IonItem
            key={i}
            onClick={() => props.onDestinationSelected(feature.geometry)}
          >
            <IonLabel>{feature.place_name}</IonLabel>
          </IonItem>
        );
      })}
    </IonList>
  );
};

export default Autocomplete;
