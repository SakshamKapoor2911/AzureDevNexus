const express = require('express');
const { verifyToken } = require('../middleware/auth');
const logger = require('../utils/logger');

const router = express.Router();

/**
 * @route   GET /api/projects
 * @desc    Get all projects with metadata
 * @access  Private
 */
router.get('/', verifyToken, async (req, res) => {
  try {
    const { organization = 'azuredevnexus' } = req.query;

    // Mock data for now - in production this would come from database
    const projects = [
      {
        id: 'project-1',
        name: 'AzureDevNexus-Platform',
        description: 'Main platform for Azure DevOps management',
        organization: organization,
        state: 'active',
        visibility: 'private',
        lastUpdateTime: new Date().toISOString(),
        createdDate: new Date(Date.now() - 2592000000).toISOString(), // 30 days ago
        defaultTeam: {
          id: 'team-1',
          name: 'Platform Team'
        },
        metrics: {
          totalWorkItems: 45,
          activeWorkItems: 12,
          completedWorkItems: 33,
          totalBuilds: 156,
          successfulBuilds: 142,
          failedBuilds: 14,
          totalReleases: 23,
          successfulReleases: 21,
          failedReleases: 2
        },
        repositories: [
          {
            id: 'repo-1',
            name: 'azuredevnexus-platform',
            defaultBranch: 'main',
            lastCommit: {
              id: 'abc123def456',
              message: 'feat: implement user authentication',
              author: 'John Doe',
              date: new Date(Date.now() - 3600000).toISOString() // 1 hour ago
            }
          }
        ],
        pipelines: [
          {
            id: 1,
            name: 'Build and Deploy',
            type: 'build',
            status: 'enabled',
            lastRun: {
              id: 1001,
              status: 'completed',
              result: 'succeeded',
              startTime: new Date(Date.now() - 3600000).toISOString(),
              finishTime: new Date(Date.now() - 3000000).toISOString()
            }
          }
        ]
      },
      {
        id: 'project-2',
        name: 'Sample-WebApp',
        description: 'Sample web application for testing',
        organization: organization,
        state: 'active',
        visibility: 'private',
        lastUpdateTime: new Date().toISOString(),
        createdDate: new Date(Date.now() - 1728000000).toISOString(), // 20 days ago
        defaultTeam: {
          id: 'team-2',
          name: 'Web Team'
        },
        metrics: {
          totalWorkItems: 23,
          activeWorkItems: 8,
          completedWorkItems: 15,
          totalBuilds: 89,
          successfulBuilds: 82,
          failedBuilds: 7,
          totalReleases: 12,
          successfulReleases: 11,
          failedReleases: 1
        },
        repositories: [
          {
            id: 'repo-2',
            name: 'sample-webapp',
            defaultBranch: 'develop',
            lastCommit: {
              id: 'def456ghi789',
              message: 'feat: add responsive design',
              author: 'Jane Smith',
              date: new Date(Date.now() - 7200000).toISOString() // 2 hours ago
            }
          }
        ],
        pipelines: [
          {
            id: 2,
            name: 'Release Pipeline',
            type: 'release',
            status: 'enabled',
            lastRun: {
              id: 1002,
              status: 'completed',
              result: 'succeeded',
              startTime: new Date(Date.now() - 7200000).toISOString(),
              finishTime: new Date(Date.now() - 6800000).toISOString()
            }
          }
        ]
      }
    ];

    logger.info({
      message: 'Projects retrieved',
      user: req.user.id,
      organization,
      count: projects.length
    });

    return res.status(200).json({
      success: true,
      data: projects
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve projects',
      error: error.message,
      user: req.user.id
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve projects',
        code: 'PROJECTS_RETRIEVAL_FAILED'
      }
    });
  }
});

/**
 * @route   GET /api/projects/:projectId
 * @desc    Get specific project details
 * @access  Private
 */
router.get('/:projectId', verifyToken, async (req, res) => {
  try {
    const { projectId } = req.params;
    const { organization = 'azuredevnexus' } = req.query;

    // Mock data for now - in production this would come from database
    const project = {
      id: projectId,
      name: 'AzureDevNexus-Platform',
      description: 'Main platform for Azure DevOps management',
      organization: organization,
      state: 'active',
      visibility: 'private',
      lastUpdateTime: new Date().toISOString(),
      createdDate: new Date(Date.now() - 2592000000).toISOString(), // 30 days ago
      defaultTeam: {
        id: 'team-1',
        name: 'Platform Team',
        members: [
          {
            id: 'user-1',
            name: 'John Doe',
            email: 'john.doe@azuredevnexus.com',
            role: 'Team Lead'
          },
          {
            id: 'user-2',
            name: 'Jane Smith',
            email: 'jane.smith@azuredevnexus.com',
            role: 'Developer'
          }
        ]
      },
      metrics: {
        totalWorkItems: 45,
        activeWorkItems: 12,
        completedWorkItems: 33,
        totalBuilds: 156,
        successfulBuilds: 142,
        failedBuilds: 14,
        totalReleases: 23,
        successfulReleases: 21,
        failedReleases: 2,
        codeCoverage: 78.5,
        technicalDebt: 12.3
      },
      repositories: [
        {
          id: 'repo-1',
          name: 'azuredevnexus-platform',
          defaultBranch: 'main',
          lastCommit: {
            id: 'abc123def456',
            message: 'feat: implement user authentication',
            author: 'John Doe',
            date: new Date(Date.now() - 3600000).toISOString() // 1 hour ago
          },
          branches: ['main', 'develop', 'feature/auth', 'hotfix/security'],
          pullRequests: {
            open: 3,
            merged: 45,
            closed: 12
          }
        }
      ],
      pipelines: [
        {
          id: 1,
          name: 'Build and Deploy',
          type: 'build',
          status: 'enabled',
          lastRun: {
            id: 1001,
            status: 'completed',
            result: 'succeeded',
            startTime: new Date(Date.now() - 3600000).toISOString(),
            finishTime: new Date(Date.now() - 3000000).toISOString()
          },
          stages: [
            {
              name: 'Build',
              status: 'completed',
              duration: 300000 // 5 minutes
            },
            {
              name: 'Test',
              status: 'completed',
              duration: 120000 // 2 minutes
            },
            {
              name: 'Deploy',
              status: 'completed',
              duration: 180000 // 3 minutes
            }
          ]
        }
      ],
      workItems: {
        byType: {
          'User Story': 15,
          'Task': 20,
          'Bug': 8,
          'Epic': 2
        },
        byState: {
          'To Do': 8,
          'In Progress': 12,
          'Done': 25
        },
        byPriority: {
          'High': 5,
          'Medium': 25,
          'Low': 15
        }
      }
    };

    logger.info({
      message: 'Project details retrieved',
      user: req.user.id,
      projectId,
      organization
    });

    return res.status(200).json({
      success: true,
      data: project
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve project details',
      error: error.message,
      user: req.user.id,
      projectId: req.params.projectId
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve project details',
        code: 'PROJECT_DETAILS_RETRIEVAL_FAILED'
      }
    });
  }
});

/**
 * @route   GET /api/projects/:projectId/metrics
 * @desc    Get project metrics and analytics
 * @access  Private
 */
router.get('/:projectId/metrics', verifyToken, async (req, res) => {
  try {
    const { projectId } = req.params;
    const { period = '30d' } = req.query;

    // Mock metrics data - in production this would be calculated from real data
    const metrics = {
      projectId,
      period,
      generatedAt: new Date().toISOString(),
      overview: {
        totalWorkItems: 45,
        activeWorkItems: 12,
        completedWorkItems: 33,
        completionRate: 73.3,
        averageCycleTime: 5.2, // days
        averageLeadTime: 8.7 // days
      },
      velocity: {
        currentSprint: 18,
        previousSprint: 15,
        averageVelocity: 16.5,
        trend: 'increasing'
      },
      quality: {
        codeCoverage: 78.5,
        technicalDebt: 12.3,
        bugDensity: 2.1, // bugs per 1000 lines
        testPassRate: 94.2
      },
      deployment: {
        totalDeployments: 23,
        successfulDeployments: 21,
        failedDeployments: 2,
        deploymentFrequency: 1.2, // per day
        meanTimeToRecovery: 2.5 // hours
      },
      build: {
        totalBuilds: 156,
        successfulBuilds: 142,
        failedBuilds: 14,
        buildSuccessRate: 91.0,
        averageBuildTime: 8.5 // minutes
      },
      trends: {
        workItemsCompleted: [12, 15, 18, 22, 25, 28, 30, 33],
        buildSuccessRate: [88, 89, 91, 90, 92, 91, 93, 91],
        codeCoverage: [72, 73, 75, 76, 77, 78, 78, 78.5],
        deploymentFrequency: [0.8, 1.0, 1.1, 1.3, 1.2, 1.4, 1.1, 1.2]
      }
    };

    logger.info({
      message: 'Project metrics retrieved',
      user: req.user.id,
      projectId,
      period
    });

    return res.status(200).json({
      success: true,
      data: metrics
    });

  } catch (error) {
    logger.error({
      message: 'Failed to retrieve project metrics',
      error: error.message,
      user: req.user.id,
      projectId: req.params.projectId
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Failed to retrieve project metrics',
        code: 'PROJECT_METRICS_RETRIEVAL_FAILED'
      }
    });
  }
});

module.exports = router;
