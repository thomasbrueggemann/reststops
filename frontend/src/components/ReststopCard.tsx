import {
  IonCard,
  IonCardHeader,
  IonCardSubtitle,
  IonCardTitle,
  IonCardContent,
  IonGrid,
  IonRow,
  IonCol,
} from "@ionic/react";
import { Reststop } from "../models/Reststop";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faRestroom,
  faGasPump,
  faParking,
  faBed,
  faCaravan,
  faBaby,
  faUtensils,
  faShower,
  faWheelchair,
  faRecycle,
  faShoppingCart,
  faInfoCircle,
  faFaucet,
  faChargingStation,
} from "@fortawesome/free-solid-svg-icons";
import Pin from "./Pin";

export interface ReststopCardProps {
  reststop: Reststop;
  numbering: number;
}

const ReststopCard: React.FC<ReststopCardProps> = (props) => {
  const hasTag = (key: string, value: string): boolean => {
    return props.reststop.tags[key] === value;
  };

  const detour = Math.ceil(props.reststop.detour_seconds / 60.0);
  const detourColor =
    detour === 0 ? "#a3be8c" : detour < 10 ? "#ebcb8b" : "#bf616a";

  const iconStyle = {
    marginRight: "10px",
  };

  return (
    <IonCard>
      <IonGrid>
        <IonRow class="ion-justify-content-between">
          <IonCol className="ion-align-self-center" size="1">
            <Pin size={40} numbering={props.numbering} />
          </IonCol>
          <IonCol>
            <IonCardHeader>
              <IonCardSubtitle>
                <b style={{ color: detourColor }}>+{detour} min</b>
              </IonCardSubtitle>
              <IonCardTitle>
                {props.reststop.name
                  ? props.reststop.name
                  : props.reststop.category === "restarea"
                  ? "Rest area"
                  : "Service area"}
              </IonCardTitle>
            </IonCardHeader>

            <IonCardContent>
              {props.reststop.description}

              {hasTag("toilets", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faRestroom}
                  style={iconStyle}
                />
              )}

              {(hasTag("highway", "services") || hasTag("amenity", "fuel")) && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faGasPump}
                  style={iconStyle}
                />
              )}

              {(hasTag("highway", "rest_area") ||
                hasTag("amenity", "parking")) && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faParking}
                  style={iconStyle}
                />
              )}

              {hasTag("tourism", "hotel") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faBed}
                  style={iconStyle}
                />
              )}

              {hasTag("caravan", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faCaravan}
                  style={iconStyle}
                />
              )}

              {hasTag("baby_change", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faBaby}
                  style={iconStyle}
                />
              )}

              {hasTag("charging_station", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faChargingStation}
                  style={iconStyle}
                />
              )}

              {(hasTag("fast_food", "yes") ||
                hasTag("restaurant", "yes") ||
                hasTag("amenity", "restaurant")) && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faUtensils}
                  style={iconStyle}
                />
              )}

              {hasTag("shower", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faShower}
                  style={iconStyle}
                />
              )}

              {hasTag("wheelchair", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faWheelchair}
                  style={iconStyle}
                />
              )}

              {hasTag("wheelchair", "no") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faWheelchair}
                  style={iconStyle}
                />
              )}

              {hasTag("waste_bucket", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faRecycle}
                  style={iconStyle}
                />
              )}

              {(hasTag("shop", "yes") || hasTag("shop", "kiosk")) && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faShoppingCart}
                  style={iconStyle}
                />
              )}

              {hasTag("tourism", "information") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faInfoCircle}
                  style={iconStyle}
                />
              )}

              {hasTag("drinking_water", "yes") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faFaucet}
                  style={iconStyle}
                />
              )}

              {hasTag("drinking_water", "no") && (
                <FontAwesomeIcon
                  size="2x"
                  fixedWidth
                  icon={faFaucet}
                  style={iconStyle}
                />
              )}

              {props.reststop.tags["opening_hours"] && (
                <p style={{ marginTop: "10px" }}>
                  <b>Open:</b>{" "}
                  <span>{props.reststop.tags["opening_hours"]}</span>
                </p>
              )}
            </IonCardContent>
          </IonCol>
        </IonRow>
      </IonGrid>
    </IonCard>
  );
};

export default ReststopCard;
