import React from "react";
import { Layer, LayerProps, Source } from "react-map-gl";

const layerStyle: LayerProps = {
  type: "circle",
  paint: {
    "circle-radius": 8,
    "circle-color": "#88c0d0",
    "circle-opacity": 1,
    "circle-stroke-color": "#fff",
    "circle-stroke-width": 1,
  },
};

export interface CurrentLocationProps {
  longitude: number;
  latitude: number;
}

const CurrentLocation: React.FC<CurrentLocationProps> = (props) => {
  return (
    <Source
      id="currentLocation"
      type="geojson"
      data={{
        type: "FeatureCollection",
        features: [
          {
            type: "Feature",
            properties: {},
            geometry: {
              type: "Point",
              coordinates: [props.longitude, props.latitude],
            },
          },
        ],
      }}
    >
      <Layer {...layerStyle} />
    </Source>
  );
};

export default CurrentLocation;
