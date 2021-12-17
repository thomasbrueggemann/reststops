import { useEffect, useState } from "react";

const useGeoLocation = () => {
  const [position, setPosition] = useState<GeolocationPosition | null>(null);

  useEffect(() => {
    if ("geolocation" in navigator) {
      navigator.geolocation.getCurrentPosition((position) => {
        setPosition(position);
      });
    }
  }, []);

  return position;
};

export default useGeoLocation;
