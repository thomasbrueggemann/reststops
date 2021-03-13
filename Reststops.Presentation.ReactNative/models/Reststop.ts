export interface Reststop {
    id:                      number;
    name:                    string;
    latitude:                number;
    longitude:               number;
    type:                    string;
    tags:                    {[key: string]: string};
    distanceInMeters:        number;
    detourDurationInSeconds: number;
}