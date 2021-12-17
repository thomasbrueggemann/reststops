import {
  IonCard,
  IonCardHeader,
  IonCardSubtitle,
  IonCardTitle,
  IonCardContent,
} from "@ionic/react";
import { Reststop } from "../models/Reststop";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRestroom, faGasPump } from "@fortawesome/free-solid-svg-icons";

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

      <IonCardContent>
        {props.reststop.description}
        <FontAwesomeIcon size="2x" fixedWidth icon={faRestroom} />
        <FontAwesomeIcon size="2x" fixedWidth icon={faGasPump} />
      </IonCardContent>
    </IonCard>
  );
};

export default ReststopCard;
