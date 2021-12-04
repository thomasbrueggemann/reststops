import {
  IonButton,
  IonButtons,
  IonContent,
  IonHeader,
  IonIcon,
  IonPage,
  IonSearchbar,
  IonTitle,
  IonToolbar,
} from "@ionic/react";
import { useEffect, useState } from "react";
import Map from "../components/Map";
import ReststopCard from "../components/ReststopCard";
import { Reststop } from "../models/Reststop";
import "./Home.css";

const Home: React.FC = () => {
  const [reststops, setReststops] = useState<Reststop[]>([]);

  const loadReststops = async () => {
    const response = await fetch(
      "https://reststops.thomasbrueggemann.com/reststops?start_lon=10.747957727309794&start_lat=53.79213882642322&end_lon=12.196588369284084&end_lat=54.059900105961155&max_detour_seconds=400"
    );

    const result: Reststop[] = await response.json();
    setReststops(result);
  };

  useEffect(() => {
    loadReststops();
  });

  return (
    <IonPage>
      <IonHeader>
        <IonToolbar>
          <IonSearchbar placeholder="What's your destination?" />
        </IonToolbar>
      </IonHeader>
      <IonContent fullscreen>
        <IonHeader collapse="condense">
          <IonToolbar>
            <IonTitle size="large">Reststops</IonTitle>
          </IonToolbar>
        </IonHeader>
        <Map />
        {reststops.map((reststop) => (
          <ReststopCard />
        ))}
      </IonContent>
    </IonPage>
  );
};

export default Home;
