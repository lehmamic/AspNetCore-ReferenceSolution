import { RouterReducerState } from '@ngrx/router-store';

// tslint:disable-next-line:no-empty-interface
export interface AppState {
}

export const initialAppState: AppState = {
};

export interface RootState {
  app: AppState;
  router: RouterReducerState;
}
