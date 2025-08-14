import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import axios from 'axios';

export interface Pipeline {
  id: string;
  name: string;
  projectId: string;
  projectName: string;
  type: string;
  status: string;
  lastRunDate: string;
  lastRunStatus: string;
  lastRunResult: string;
  url: string;
}

export interface PipelineRun {
  id: string;
  pipelineId: string;
  status: string;
  result: string | null;
  queueTime: string;
  startTime: string | null;
  finishTime: string | null;
  duration: number | null;
  requestedBy: {
    id: string;
    displayName: string;
    uniqueName: string;
  };
  sourceBranch: string;
  variables: Record<string, any>;
}

export interface PipelinesState {
  pipelines: Pipeline[];
  currentPipeline: Pipeline | null;
  pipelineRuns: PipelineRun[];
  isLoading: boolean;
  error: string | null;
}

const initialState: PipelinesState = {
  pipelines: [],
  currentPipeline: null,
  pipelineRuns: [],
  isLoading: false,
  error: null,
};

export const fetchPipelines = createAsyncThunk(
  'pipelines/fetchPipelines',
  async (_, { rejectWithValue }) => {
    try {
      const response = await axios.get('/api/pipelines');
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch pipelines');
    }
  }
);

export const fetchPipelineDetails = createAsyncThunk(
  'pipelines/fetchPipelineDetails',
  async (pipelineId: string, { rejectWithValue }) => {
    try {
      const response = await axios.get(`/api/pipelines/${pipelineId}`);
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch pipeline details');
    }
  }
);

export const fetchPipelineRuns = createAsyncThunk(
  'pipelines/fetchPipelineRuns',
  async (pipelineId: string, { rejectWithValue }) => {
    try {
      const response = await axios.get(`/api/pipelines/${pipelineId}/runs`);
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch pipeline runs');
    }
  }
);

export const triggerPipelineRun = createAsyncThunk(
  'pipelines/triggerPipelineRun',
  async ({ pipelineId, branch, variables }: { pipelineId: string; branch?: string; variables?: Record<string, any> }, { rejectWithValue }) => {
    try {
      const response = await axios.post(`/api/pipelines/${pipelineId}/run`, {
        branch,
        variables
      });
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to trigger pipeline run');
    }
  }
);

const pipelinesSlice = createSlice({
  name: 'pipelines',
  initialState,
  reducers: {
    setCurrentPipeline: (state, action: PayloadAction<Pipeline>) => {
      state.currentPipeline = action.payload;
    },
    clearCurrentPipeline: (state) => {
      state.currentPipeline = null;
      state.pipelineRuns = [];
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch Pipelines
      .addCase(fetchPipelines.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchPipelines.fulfilled, (state, action) => {
        state.isLoading = false;
        state.pipelines = action.payload;
        state.error = null;
      })
      .addCase(fetchPipelines.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Fetch Pipeline Details
      .addCase(fetchPipelineDetails.fulfilled, (state, action) => {
        state.currentPipeline = action.payload;
        state.error = null;
      })
      .addCase(fetchPipelineDetails.rejected, (state, action) => {
        state.error = action.payload as string;
      })
      // Fetch Pipeline Runs
      .addCase(fetchPipelineRuns.fulfilled, (state, action) => {
        state.pipelineRuns = action.payload;
        state.error = null;
      })
      .addCase(fetchPipelineRuns.rejected, (state, action) => {
        state.error = action.payload as string;
      })
      // Trigger Pipeline Run
      .addCase(triggerPipelineRun.fulfilled, (state, action) => {
        state.pipelineRuns.unshift(action.payload);
        state.error = null;
      })
      .addCase(triggerPipelineRun.rejected, (state, action) => {
        state.error = action.payload as string;
      });
  },
});

export const { setCurrentPipeline, clearCurrentPipeline, clearError } = pipelinesSlice.actions;
export default pipelinesSlice.reducer;
