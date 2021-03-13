import Place from "../models/Place";
import { buildContext, ReducerAction } from "./Context";

export interface DestinationState {
	destination: Place | null;
}

export enum DestinationActions {
	SET
}

const initialState: DestinationState = {
	destination: null
};

// reducer actions to mutate state
function reducer(
	state: DestinationState,
	action: DestinationState & ReducerAction<DestinationActions>
): DestinationState {
	switch (action.type) {
		case DestinationActions.SET:
			return {
				...state,
				destination: action.destination
			};
	}
}

export default buildContext<DestinationState, DestinationActions>(initialState, reducer);
