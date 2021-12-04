import React, { useState } from "react";
import ReactMapGL from "react-map-gl";

const Map: React.FC = () => {
  const [viewport, setViewport] = useState({
    latitude: 37.7577,
    longitude: -122.4376,
    zoom: 8,
  });

  return (
    <ReactMapGL
      {...viewport}
      mapboxApiAccessToken="pk.eyJ1IjoiYmx1ZWdnZW1hbm4iLCJhIjoiY2tiZW1xMGloMG40cjJzbm9jZDB2NGxwdyJ9.GEBzqnPxTpcFo7HLC8EzRQ"
      width="100%"
      height="300px"
      onViewportChange={(viewport: any) => setViewport(viewport)}
    />
  );
};

export default Map;
