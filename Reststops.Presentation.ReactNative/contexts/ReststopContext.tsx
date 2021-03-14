import { Reststop } from "../models/Reststop";
import { buildContext, ReducerAction } from "./Context";

export interface ReststopState {
	selected: Reststop | null;
}

export enum ReststopActions {
	SET_SELECTED
}

const initialState: ReststopState = {
	selected: null
};

// reducer actions to mutate state
function reducer(
	state: ReststopState,
	action: ReststopState & ReducerAction<ReststopActions>
): ReststopState {
	switch (action.type) {
		case ReststopActions.SET_SELECTED:
			return {
				...state,
				selected: action.selected
			};
	}
}

export default buildContext<ReststopState, ReststopActions>(initialState, reducer);
