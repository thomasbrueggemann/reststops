import {
  IonContent,
  IonHeader,
  IonPage,
  IonProgressBar,
  IonSearchbar,
  IonTitle,
  IonToolbar,
} from "@ionic/react";
import { useEffect, useState } from "react";
import Map from "../components/Map";
import ReststopCard from "../components/ReststopCard";
import { Reststop } from "../models/Reststop";
import "./Home.css";
import Autocomplete from "../components/Autocomplete";
import { Geometry } from "../models/GeocodingResult";
import useGeoLocation from "../hooks/useLocation";

interface ReststopsResponse {
  reststops: Reststop[];
  route: string;
  bbox: number[];
}

const Home: React.FC = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const [reststops, setReststops] = useState<Reststop[]>([]);
  const [route, setRoute] = useState<string | null>(null);
  const [bbox, setBbox] = useState<number[]>([]);
  const [searchText, setSearchText] = useState<string | null>(null);
  const [destination, setDestination] = useState<Geometry | null>(null);
  const geolocation = useGeoLocation();

  const loadReststops = async (
    geolocation: GeolocationPosition | null,
    destination: Geometry | null
  ) => {
    if (destination === null || geolocation === null) {
      return;
    }

    setLoading(true);
    const host = "https://reststops.thomasbrueggemann.com";
    const response = await fetch(
      `${host}/reststops?start_lon=${geolocation.coords.longitude}&start_lat=${geolocation.coords.latitude}` +
        `&end_lon=${destination.coordinates[0]}&end_lat=${destination.coordinates[1]}&max_detour_seconds=400`
    );

    const result: ReststopsResponse = await response.json();

    setLoading(false);

    setReststops(result.reststops);
    setRoute(result.route);
    setBbox(result.bbox);
  };

  useEffect(() => {
    loadReststops(geolocation, destination);
  }, [geolocation, destination]);

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonSearchbar
            placeholder="What's your destination?"
            onIonChange={(e) => {
              setSearchText(e.detail.value!);
              setReststops([]);
              setRoute(null);
              setBbox([]);
            }}
          />
        </IonToolbar>
      </IonHeader>
      <IonContent fullscreen>
        <IonHeader collapse="condense">
          <IonToolbar>
            <IonTitle size="large">Reststops</IonTitle>
          </IonToolbar>
        </IonHeader>

        {!loading && reststops.length === 0 && searchText === null && (
          <div>Please select a destination</div>
        )}

        {!loading &&
          reststops.length === 0 &&
          searchText !== null &&
          searchText.length >= 3 && (
            <Autocomplete
              destination={searchText}
              onDestinationSelected={(destination) =>
                setDestination(destination)
              }
            />
          )}

        {reststops.length > 0 && (
          <Map
            reststops={reststops}
            route={route}
            bbox={bbox}
            currentLocation={geolocation}
          />
        )}

        {loading && <IonProgressBar type="indeterminate"></IonProgressBar>}

        {reststops.map((reststop, i) => (
          <ReststopCard key={i} reststop={reststop} />
        ))}
      </IonContent>
    </IonPage>
  );
};

export default Home;
