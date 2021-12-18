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

  useEffect(() => {
    const abortController = new AbortController();

    const getGeocodingResults = async (searchText: string) => {
      if (searchText && searchText.length < 3) {
        return;
      }

      const baseUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places";

      try {
        const response = await fetch(
          `${baseUrl}/${encodeURIComponent(
            searchText
          )}.json?autocomplete=true&fuzzyMatch=true&access_token=${MAPBOX_TOKEN}`,
          { signal: abortController.signal }
        );

        const result: GeocodingResult = await response.json();
        setFeatures(result.features);
      } catch {}
    };

    getGeocodingResults(props.destination);

    return () => {
      abortController.abort();
    };
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
