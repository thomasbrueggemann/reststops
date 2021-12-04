import {
  IonCard,
  IonCardHeader,
  IonCardSubtitle,
  IonCardTitle,
  IonCardContent,
} from "@ionic/react";
import { Reststop } from "../models/Reststop";

export interface ReststopCardProps {
  reststop: Reststop;
}

const ReststopCard: React.FC<ReststopCardProps> = (props) => {
  return (
    <IonCard>
      <IonCardHeader>
        <IonCardSubtitle>
          {props.reststop.category === "rest_area" ? "Reststop" : "Gas-Station"}
        </IonCardSubtitle>
        <IonCardTitle>{props.reststop.name}</IonCardTitle>
      </IonCardHeader>

      <IonCardContent>{props.reststop.description}</IonCardContent>
    </IonCard>
  );
};

export default ReststopCard;
