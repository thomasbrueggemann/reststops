import React, { createContext, useReducer } from "react";

export interface ReducerAction<T> {
	type: T;
}

export function buildContext<T, A>(initialState: T, reducer: (state: T, action: T & ReducerAction<A>) => T) {
	// create a new context
	const Context = createContext<{
		state: T;
		dispatch?: React.Dispatch<T & ReducerAction<A>>;
	}>({ state: initialState });

	function ContextProvider(props: any) {
		// manage state my react reducer
		const [state, dispatch] = useReducer(reducer, initialState);

		// populate the provider with the current state and a dispatcher function
		return <Context.Provider value={{ state, dispatch }}>{props.children}</Context.Provider>;
	}

	return { Context, ContextProvider, ContextConsumer: Context.Consumer };
}
