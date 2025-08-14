import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import projectsReducer from './slices/projectsSlice';
import pipelinesReducer from './slices/pipelinesSlice';
import uiReducer from './slices/uiSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    projects: projectsReducer,
    pipelines: pipelinesReducer,
    ui: uiReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['persist/PERSIST'],
      },
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
