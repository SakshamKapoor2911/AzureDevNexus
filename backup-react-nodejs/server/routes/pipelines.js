const express = require('express');
const { verifyToken } = require('../middleware/auth');
const logger = require('../utils/logger');

const router = express.Router();

/**
 * @route   GET /api/pipelines
 * @desc    Get all pipelines across projects
 * @access  Private
 */
router.get('/', verifyToken, async (req, res) => {
  try {
    const { organization = 'azuredevnexus', status, type } = req.query;

    // Mock data for now - in production this would come from database
    const pipelines = [
      {
        id: 1,
        name: 'Build and Deploy',
        projectId: 'project-1',
        projectName: 'AzureDevNexus-Platform',
        type: 'build',
        status: 'enabled',
        folder: '\\',
        revision: 1,
        url: `https://dev.azure.com/${organization}/AzureDevNexus-Platform/_apis/pipelines/1`,
        webUrl: `https://dev.azure.com/${organization}/AzureDevNexus-Platform/_build?definitionId=1`,
        createdDate: new Date(Date.now() - 86400000).toISOString(), // 1 day ago
        updatedDate: new Date().toISOString(),
        lastRun: {
          id: 1001,
          status: 'completed',
          result: 'succeeded',
          startTime: new Date(Date.now() - 3600000).toISOString(),
          finishTime: new Date(Date.now() - 3000000).toISOString(),
          duration: 600000 // 10 minutes
        },
        metrics: {
          totalRuns: 156,
          successfulRuns: 142,
          failedRuns: 14,
          successRate: 91.0,
          averageDuration: 480000 // 8 minutes
        }
      },
      {
        id: 2,
        name: 'Release Pipeline',
        projectId: 'project-1',
        projectName: 'AzureDevNexus-Platform',
        type: 'release',
        status: 'enabled',
        folder: '\\Releases',
        revision: 1,
        url: `https://dev.azure.com/${organization}/AzureDevNexus-Platform/_apis/pipelines/2`,
        webUrl: `https://vsrm.dev.azure.com/${organization}/AzureDevNexus-Platform/_release?definitionId=2`,
        createdDate: new Date(Date.now() - 172800000).toISOString(), // 2 days ago
        updatedDate: new Date().toISOString(),
        lastRun: {
          id: 1002,
          status: 'completed',
          result: 'succeeded',
          startTime: new Date(Date.now() - 7200000).toISOString(),
          finishTime: new Date(Date.now() - 6800000).toISOString(),
          duration: 400000 // 6.7 minutes
        },
        metrics: {
          totalRuns: 23,
          successfulRuns: 21,
          failedRuns: 2,
          successRate: 91.3,
          averageDuration: 360000 // 6 minutes
        }
      },
      {
        id: 3,
        name: 'Web App Build',
        projectId: 'project-2',
        projectName: 'Sample-WebApp',
        type: 'build',
        status: 'enabled',
        folder: '\\',
        revision: 1,
        url: `https://dev.azure.com/${organization}/Sample-WebApp/_apis/pipelines/3`,
        webUrl: `https://dev.azure.com/${organization}/Sample-WebApp/_build?definitionId=3`,
        createdDate: new Date(Date.now() - 259200000).toISOString(), // 3 days ago
        updatedDate: new Date().toISOString(),
        lastRun: {
          id: 1003,
          status: 'inProgress',
          result: null,
          startTime: new Date(Date.now() - 1800000).toISOString(),
          finishTime: null,
          duration: 1800000 // 30 minutes (ongoing)
        },
        metrics: {
          totalRuns: 89,
          successfulRuns: 82,
          failedRuns: 7,
          successRate: 92.1,
          averageDuration: 300000 // 5 minutes
        }
      }
    ];

    // Apply filters if provided
    let filteredPipelines = pipelines;
    if (status) {
      filteredPipelines = filteredPipelines.filter(p => p.status === status);
    }
    if (type) {
      filteredPipelines = filteredPipelines.filter(p => p.type === type);
    }

    logger.info({
      message: 'Pipelines retrieved',
      user: req.user.id,
      organization,
      count: filteredPipelines.length,
      filters: { status, type }
    });

    return res.status(200).json({
      success: true,
      data: filteredPipelines
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve pipelines',
      error: error.message,
      user: req.user.id
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve pipelines',
        code: 'PIPELINES_RETRIEVAL_FAILED'
      }
    });
  }
});

/**
 * @route   GET /api/pipelines/:pipelineId
 * @desc    Get specific pipeline details
 * @access  Private
 */
router.get('/:pipelineId', verifyToken, async (req, res) => {
  try {
    const { pipelineId } = req.params;
    const { organization = 'azuredevnexus' } = req.query;

    // Mock data for now - in production this would come from database
    const pipeline = {
      id: parseInt(pipelineId),
      name: 'Build and Deploy',
      projectId: 'project-1',
      projectName: 'AzureDevNexus-Platform',
      type: 'build',
      status: 'enabled',
      folder: '\\',
      revision: 1,
      url: `https://dev.azure.com/${organization}/AzureDevNexus-Platform/_apis/pipelines/${pipelineId}`,
      webUrl: `https://dev.azure.com/${organization}/AzureDevNexus-Platform/_build?definitionId=${pipelineId}`,
      createdDate: new Date(Date.now() - 86400000).toISOString(),
      updatedDate: new Date().toISOString(),
      description: 'Main build and deployment pipeline for the AzureDevNexus platform',
      repository: {
        id: 'repo-1',
        name: 'azuredevnexus-platform',
        type: 'Git',
        defaultBranch: 'main'
      },
      triggers: [
        {
          type: 'continuousIntegration',
          branchFilters: ['main', 'develop'],
          pathFilters: ['src/**', 'tests/**']
        },
        {
          type: 'pullRequest',
          branchFilters: ['main', 'develop'],
          pathFilters: ['src/**', 'tests/**']
        }
      ],
      variables: [
        {
          name: 'BUILD_CONFIGURATION',
          value: 'Release',
          isSecret: false
        },
        {
          name: 'DEPLOYMENT_ENVIRONMENT',
          value: 'Development',
          isSecret: false
        }
      ],
      stages: [
        {
          name: 'Build',
          order: 1,
          status: 'completed',
          duration: 300000, // 5 minutes
          tasks: [
            {
              name: 'Restore NuGet packages',
              status: 'completed',
              duration: 45000 // 45 seconds
            },
            {
              name: 'Build solution',
              status: 'completed',
              duration: 180000 // 3 minutes
            },
            {
              name: 'Run unit tests',
              status: 'completed',
              duration: 75000 // 1.25 minutes
            }
          ]
        },
        {
          name: 'Test',
          order: 2,
          status: 'completed',
          duration: 120000, // 2 minutes
          tasks: [
            {
              name: 'Run integration tests',
              status: 'completed',
              duration: 90000 // 1.5 minutes
            },
            {
              name: 'Code coverage analysis',
              status: 'completed',
              duration: 30000 // 30 seconds
            }
          ]
        },
        {
          name: 'Deploy',
          order: 3,
          status: 'completed',
          duration: 180000, // 3 minutes
          tasks: [
            {
              name: 'Deploy to development',
              status: 'completed',
              duration: 120000 // 2 minutes
            },
            {
              name: 'Health check',
              status: 'completed',
              duration: 60000 // 1 minute
            }
          ]
        }
      ],
      lastRun: {
        id: 1001,
        status: 'completed',
        result: 'succeeded',
        startTime: new Date(Date.now() - 3600000).toISOString(),
        finishTime: new Date(Date.now() - 3000000).toISOString(),
        duration: 600000, // 10 minutes
        requestedBy: {
          displayName: 'John Doe',
          uniqueName: 'john.doe@azuredevnexus.com'
        },
        sourceBranch: 'refs/heads/main',
        sourceVersion: 'abc123def456',
        commitMessage: 'feat: implement user authentication'
      },
      metrics: {
        totalRuns: 156,
        successfulRuns: 142,
        failedRuns: 14,
        successRate: 91.0,
        averageDuration: 480000, // 8 minutes
        totalDuration: 74880000, // 20.8 hours
        lastSuccess: new Date(Date.now() - 3600000).toISOString(),
        lastFailure: new Date(Date.now() - 86400000).toISOString() // 1 day ago
      },
      recentRuns: [
        {
          id: 1001,
          buildNumber: '20231115.1',
          status: 'completed',
          result: 'succeeded',
          startTime: new Date(Date.now() - 3600000).toISOString(),
          finishTime: new Date(Date.now() - 3000000).toISOString(),
          duration: 600000
        },
        {
          id: 1000,
          buildNumber: '20231114.5',
          status: 'completed',
          result: 'succeeded',
          startTime: new Date(Date.now() - 86400000).toISOString(),
          finishTime: new Date(Date.now() - 82800000).toISOString(),
          duration: 540000
        },
        {
          id: 999,
          buildNumber: '20231114.4',
          status: 'completed',
          result: 'failed',
          startTime: new Date(Date.now() - 172800000).toISOString(),
          finishTime: new Date(Date.now() - 169200000).toISOString(),
          duration: 360000
        }
      ]
    };

    logger.info({
      message: 'Pipeline details retrieved',
      user: req.user.id,
      pipelineId,
      organization
    });

    return res.status(200).json({
      success: true,
      data: pipeline
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve pipeline details',
      error: error.message,
      user: req.user.id,
      pipelineId: req.params.pipelineId
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve pipeline details',
        code: 'PIPELINE_DETAILS_RETRIEVAL_FAILED'
      }
    });
  }
});

/**
 * @route   POST /api/pipelines/:pipelineId/run
 * @desc    Trigger a pipeline run
 * @access  Private
 */
router.post('/:pipelineId/run', verifyToken, async (req, res) => {
  try {
    const { pipelineId } = req.params;
    const { branch = 'main', variables = {} } = req.body;

    // In a real implementation, you would call the Azure DevOps API to trigger a run
    // POST https://dev.azure.com/{organization}/{project}/_apis/pipelines/{pipelineId}/runs?api-version=7.0

    // Mock response for now
    const runId = Math.floor(Math.random() * 10000) + 1000;
    const run = {
      id: runId,
      pipelineId: parseInt(pipelineId),
      status: 'queued',
      result: null,
      queueTime: new Date().toISOString(),
      startTime: null,
      finishTime: null,
      duration: null,
      requestedBy: {
        id: req.user.id,
        displayName: req.user.username,
        uniqueName: req.user.email
      },
      sourceBranch: branch,
      variables: variables
    };

    logger.info({
      message: 'Pipeline run triggered',
      user: req.user.id,
      pipelineId,
      runId,
      branch,
      variables
    });

    return res.status(200).json({
      success: true,
      message: 'Pipeline run triggered successfully',
      data: run
    });

  } catch (error) {
    logger.error({
      message: 'Failed to trigger pipeline run',
      error: error.message,
      user: req.user.id,
      pipelineId: req.params.pipelineId
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to trigger pipeline run',
        code: 'PIPELINE_RUN_TRIGGER_FAILED'
      }
    });
  }
});

/**
 * @route   GET /api/pipelines/:pipelineId/runs
 * @desc    Get pipeline runs history
 * @access  Private
 */
router.get('/:pipelineId/runs', verifyToken, async (req, res) => {
  try {
    const { pipelineId } = req.params;
    const { organization = 'azuredevnexus' } = req.query;
    const { status, result, $top = 50 } = req.query;

    // Mock data for now - in production this would come from database
    const runs = [
      {
        id: 1001,
        pipelineId: parseInt(pipelineId),
        buildNumber: '20231115.1',
        status: 'completed',
        result: 'succeeded',
        queueTime: new Date(Date.now() - 3600000).toISOString(),
        startTime: new Date(Date.now() - 3500000).toISOString(),
        finishTime: new Date(Date.now() - 3000000).toISOString(),
        duration: 600000, // 10 minutes
        requestedBy: {
          displayName: 'John Doe',
          uniqueName: 'john.doe@azuredevnexus.com'
        },
        sourceBranch: 'refs/heads/main',
        sourceVersion: 'abc123def456',
        commitMessage: 'feat: implement user authentication',
        stages: [
          {
            name: 'Build',
            status: 'completed',
            duration: 300000
          },
          {
            name: 'Test',
            status: 'completed',
            duration: 120000
          },
          {
            name: 'Deploy',
            status: 'completed',
            duration: 180000
          }
        ]
      },
      {
        id: 1000,
        pipelineId: parseInt(pipelineId),
        buildNumber: '20231114.5',
        status: 'completed',
        result: 'succeeded',
        queueTime: new Date(Date.now() - 86400000).toISOString(),
        startTime: new Date(Date.now() - 85800000).toISOString(),
        finishTime: new Date(Date.now() - 82800000).toISOString(),
        duration: 540000, // 9 minutes
        requestedBy: {
          displayName: 'Jane Smith',
          uniqueName: 'jane.smith@azuredevnexus.com'
        },
        sourceBranch: 'refs/heads/develop',
        sourceVersion: 'def456ghi789',
        commitMessage: 'fix: resolve authentication bug',
        stages: [
          {
            name: 'Build',
            status: 'completed',
            duration: 270000
          },
          {
            name: 'Test',
            status: 'completed',
            duration: 150000
          },
          {
            name: 'Deploy',
            status: 'completed',
            duration: 120000
          }
        ]
      }
    ];

    // Apply filters if provided
    let filteredRuns = runs;
    if (status) {
      filteredRuns = filteredRuns.filter(r => r.status === status);
    }
    if (result) {
      filteredRuns = filteredRuns.filter(r => r.result === result);
    }

    logger.info({
      message: 'Pipeline runs retrieved',
      user: req.user.id,
      pipelineId,
      count: filteredRuns.length,
      filters: { status, result, $top }
    });

    return res.status(200).json({
      success: true,
      data: filteredRuns
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve pipeline runs',
      error: error.message,
      user: req.user.id,
      pipelineId: req.params.pipelineId
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve pipeline runs',
        code: 'PIPELINE_RUNS_RETRIEVAL_FAILED'
      }
    });
  }
});

module.exports = router;
