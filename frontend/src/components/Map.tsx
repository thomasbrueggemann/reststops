/* eslint-disable react-hooks/exhaustive-deps */
import React, { useEffect, useState } from "react";
import ReactMapGL, {
  FlyToInterpolator,
  Layer,
  LayerProps,
  Marker,
  Source,
  WebMercatorViewport,
} from "react-map-gl";
import { Reststop } from "../models/Reststop";
import Pin from "./Pin";
import polyline from "@mapbox/polyline";
import { WebMercatorViewportOptions } from "@math.gl/web-mercator/src/web-mercator-viewport";
import { MAPBOX_TOKEN } from "../constants";
import CurrentLocation from "./CurrentLocation";

import mapboxgl from "mapbox-gl"; // This is a dependency of react-map-gl even if you didn't explicitly install it

// @ts-ignore
mapboxgl.workerClass =
  // eslint-disable-next-line import/no-webpack-loader-syntax
  require("worker-loader!mapbox-gl/dist/mapbox-gl-csp-worker").default;

const mapHeight: number = 300;

export interface MapProps {
  reststops: Reststop[];
  route: string | null;
  bbox: number[];
  currentLocation: GeolocationPosition | null;
}

const Map: React.FC<MapProps> = (props) => {
  const [viewport, setViewport] = useState({
    latitude: 50.7577,
    longitude: -13.4376,
    zoom: 3,
    transitionDuration: 1111,
    transitionInterpolator: new FlyToInterpolator(),
  });

  useEffect(() => {
    if (props.bbox.length > 0) {
      const mercatorViewportOptions: WebMercatorViewportOptions = {
        width: window.innerWidth,
        height: mapHeight,
        latitude: viewport.latitude,
        longitude: viewport.longitude,
        zoom: viewport.zoom,
      };

      const newViewport = new WebMercatorViewport(
        mercatorViewportOptions
      ).fitBounds([
        [props.bbox[0], props.bbox[1]],
        [props.bbox[2], props.bbox[3]],
      ]);

      setViewport({
        ...viewport,
        ...newViewport,
      });
    }
  }, [props.bbox]);

  const layerStyle: LayerProps = {
    type: "line",
    paint: {
      "line-width": 5,
      "line-color": "#ebcb8b",
      "line-opacity": 0.65,
    },
    layout: {
      "line-join": "round",
    },
  };

  return (
    <ReactMapGL
      {...viewport}
      mapboxApiAccessToken={MAPBOX_TOKEN}
      width="100%"
      height={mapHeight + "px"}
      mapStyle="mapbox://styles/mapbox/dark-v9"
      onViewportChange={(viewport: any) => setViewport(viewport)}
    >
      {props.route && (
        <Source
          id="route"
          type="geojson"
          data={{
            type: "FeatureCollection",
            features: [
              {
                type: "Feature",
                properties: {},
                geometry: {
                  type: "LineString",
                  coordinates: polyline
                    .decode(props.route, 5)
                    .map((coord) => coord.reverse()),
                },
              },
            ],
          }}
        >
          <Layer {...layerStyle} />
        </Source>
      )}

      {props.currentLocation && (
        <CurrentLocation
          longitude={props.currentLocation.coords.longitude}
          latitude={props.currentLocation.coords.latitude}
        />
      )}

      {props.reststops.map((reststop, i) => {
        return (
          <Marker
            key={i}
            longitude={reststop.location[0]}
            latitude={reststop.location[1]}
            offsetTop={-20}
            offsetLeft={-10}
          >
            <Pin size={20} />
          </Marker>
        );
      })}
    </ReactMapGL>
  );
};

export default Map;
