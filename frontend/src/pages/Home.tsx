import {
  IonContent,
  IonHeader,
  IonPage,
  IonProgressBar,
  IonRefresher,
  IonRefresherContent,
  IonSearchbar,
  IonToolbar,
  RefresherEventDetail,
} from "@ionic/react";
import { useEffect, useState } from "react";
import Map from "../components/Map";
import ReststopCard from "../components/ReststopCard";
import { Reststop } from "../models/Reststop";
import "./Home.css";
import Autocomplete from "../components/Autocomplete";
import { Geometry } from "../models/GeocodingResult";
import useGeoLocation from "../hooks/useLocation";
import useLocalStorage from "../hooks/useLocalStorage";

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
  const [destination, setDestination] = useLocalStorage("destination", null);
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

  const handleRefresh = (event: CustomEvent<RefresherEventDetail>) => {
    loadReststops(geolocation, destination);
    event.detail.complete();
  };

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
        {!loading && reststops.length === 0 && searchText === null && (
          <div style={{ textAlign: "center", marginTop: "40px" }}>
            🚏 Please find a destination
          </div>
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

        <IonRefresher slot="fixed" onIonRefresh={handleRefresh}>
          <IonRefresherContent></IonRefresherContent>
        </IonRefresher>

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
          <ReststopCard key={i} numbering={i + 1} reststop={reststop} />
        ))}
      </IonContent>
    </IonPage>
  );
};

export default Home;
