import { routerReducer } from '@ngrx/router-store';
import {
  ActionReducerMap,
  createReducer,
  MetaReducer,
  on,
} from '@ngrx/store';

import { environment } from '../environments/environment';

import { dummy } from './app.actions';
import { initialAppState, RootState } from './app.state';

const appReducer = createReducer(initialAppState,
  on(dummy, state => state),
);

export const reducers: ActionReducerMap<RootState> = {
  app: appReducer,
  router: routerReducer,
};

export const metaReducers: MetaReducer<RootState>[] = !environment.production ? [] : [];
