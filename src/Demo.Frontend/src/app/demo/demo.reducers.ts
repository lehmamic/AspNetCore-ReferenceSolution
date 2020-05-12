import {
  createReducer,
  on,
} from '@ngrx/store';

import { dummy } from './demo.actions';
import { initialDemoState } from './demo.state';

export const demoReducer = createReducer(initialDemoState,
  on(dummy, state => state),
);
