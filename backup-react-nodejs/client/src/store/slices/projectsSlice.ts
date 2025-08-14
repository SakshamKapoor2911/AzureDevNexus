import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import axios from 'axios';

export interface Project {
  id: string;
  name: string;
  description: string;
  url: string;
  state: string;
  visibility: string;
  lastUpdateTime: string;
  defaultTeam: {
    id: string;
    name: string;
  };
}

export interface ProjectMetrics {
  projectId: string;
  period: string;
  generatedAt: string;
  overview: {
    totalWorkItems: number;
    activeWorkItems: number;
    completedWorkItems: number;
    completionRate: number;
    averageCycleTime: number;
    averageLeadTime: number;
  };
  velocity: {
    currentSprint: number;
    previousSprint: number;
    averageVelocity: number;
    trend: string;
  };
  quality: {
    codeCoverage: number;
    technicalDebt: number;
    bugDensity: number;
    testPassRate: number;
  };
  deployment: {
    totalDeployments: number;
    successfulDeployments: number;
    failedDeployments: number;
    deploymentFrequency: number;
    meanTimeToRecovery: number;
  };
  build: {
    totalBuilds: number;
    successfulBuilds: number;
    failedBuilds: number;
    buildSuccessRate: number;
    averageBuildTime: number;
  };
  trends: {
    workItemsCompleted: number[];
    buildSuccessRate: number[];
    codeCoverage: number[];
    deploymentFrequency: number[];
  };
}

export interface ProjectsState {
  projects: Project[];
  currentProject: Project | null;
  projectMetrics: ProjectMetrics | null;
  isLoading: boolean;
  error: string | null;
}

const initialState: ProjectsState = {
  projects: [],
  currentProject: null,
  projectMetrics: null,
  isLoading: false,
  error: null,
};

export const fetchProjects = createAsyncThunk(
  'projects/fetchProjects',
  async (organization?: string, { rejectWithValue }) => {
    try {
      const response = await axios.get('/api/azure-devops/projects', {
        params: { organization }
      });
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch projects');
    }
  }
);

export const fetchProjectDetails = createAsyncThunk(
  'projects/fetchProjectDetails',
  async (projectId: string, { rejectWithValue }) => {
    try {
      const response = await axios.get(`/api/projects/${projectId}`);
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch project details');
    }
  }
);

export const fetchProjectMetrics = createAsyncThunk(
  'projects/fetchProjectMetrics',
  async ({ projectId, period }: { projectId: string; period?: string }, { rejectWithValue }) => {
    try {
      const response = await axios.get(`/api/projects/${projectId}/metrics`, {
        params: { period }
      });
      return response.data.data;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.error?.message || 'Failed to fetch project metrics');
    }
  }
);

const projectsSlice = createSlice({
  name: 'projects',
  initialState,
  reducers: {
    setCurrentProject: (state, action: PayloadAction<Project>) => {
      state.currentProject = action.payload;
    },
    clearCurrentProject: (state) => {
      state.currentProject = null;
      state.projectMetrics = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch Projects
      .addCase(fetchProjects.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchProjects.fulfilled, (state, action) => {
        state.isLoading = false;
        state.projects = action.payload;
        state.error = null;
      })
      .addCase(fetchProjects.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      // Fetch Project Details
      .addCase(fetchProjectDetails.fulfilled, (state, action) => {
        state.currentProject = action.payload;
        state.error = null;
      })
      .addCase(fetchProjectDetails.rejected, (state, action) => {
        state.error = action.payload as string;
      })
      // Fetch Project Metrics
      .addCase(fetchProjectMetrics.fulfilled, (state, action) => {
        state.projectMetrics = action.payload;
        state.error = null;
      })
      .addCase(fetchProjectMetrics.rejected, (state, action) => {
        state.error = action.payload as string;
      });
  },
});

export const { setCurrentProject, clearCurrentProject, clearError } = projectsSlice.actions;
export default projectsSlice.reducer;
