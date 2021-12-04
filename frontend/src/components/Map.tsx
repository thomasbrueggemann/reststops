import React, { useEffect, useState } from "react";
import ReactMapGL, {
  FlyToInterpolator,
  GeolocateControl,
  Marker,
  WebMercatorViewport,
} from "react-map-gl";
import { Reststop } from "../models/Reststop";

const geolocateStyle = {
  top: 0,
  left: 0,
  margin: 10,
};
const positionOptions = { enableHighAccuracy: true };

export interface MapProps {
  reststops: Reststop[];
}

const Map: React.FC<MapProps> = (props) => {
  const [viewport, setViewport] = useState({
    latitude: 50.7577,
    longitude: -13.4376,
    zoom: 3,
    transitionDuration: 5000,
    transitionInterpolator: new FlyToInterpolator(),
  });

  /*useEffect(() => {
    const { longitude, latitude, zoom } = new WebMercatorViewport(
      viewport
    ).fitBounds(
      props.reststops.map((r) => [r.location[0], r.location]),
      {
        padding: 20,
        offset: [0, -100],
      }
    );
  }, [props.reststops, viewport]);*/

  return (
    <ReactMapGL
      {...viewport}
      mapboxApiAccessToken="pk.eyJ1IjoiYmx1ZWdnZW1hbm4iLCJhIjoiY2tiZW1xMGloMG40cjJzbm9jZDB2NGxwdyJ9.GEBzqnPxTpcFo7HLC8EzRQ"
      width="100%"
      height="300px"
      mapStyle="mapbox://styles/mapbox/dark-v9"
      onViewportChange={(viewport: any) => setViewport(viewport)}
    >
      <GeolocateControl
        style={geolocateStyle}
        positionOptions={positionOptions}
        trackUserLocation
        auto
      />

      {props.reststops.map((reststop) => {
        return (
          <Marker
            longitude={reststop.location[0]}
            latitude={reststop.location[1]}
            offsetTop={-20}
            offsetLeft={-10}
          ></Marker>
        );
      })}
    </ReactMapGL>
  );
};

export default Map;
