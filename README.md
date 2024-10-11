# com.unity.render-pipelines.universal.mini

## Introduction

- 引入管线前需要为引擎添加 WX_PERFORMANCE_MODE 的宏定义，基于 urp 14.0.9 定制，引擎环境为 Tuanjie 1.2.3+
- 专为微信小游戏项目而定制，主题思路是轻量，简单，剔除无用的代码，判断，变体
- 强制关闭一些在小游戏平台使用时负载较高的特性，例如 realtime additional light，2 级以上的级联阴影，后处理等
- 定制渲染管线逻辑，干掉无用的 rt，例如 additional shadow map，empty shadow map，等
- 修复一些应用中的问题，例如在使用 render scale 的情况下 depth attachement 会成倍增加等
- 提供一套更精简的着色模型作为基础 shader，现在切到该管线后，默认 shading 使用的是 SoFunny/Mini/MiniLit，原有 URP shaders 继续提供
- 强制使用 single camera 方案，如果场景中出现多个 camera，只有 depth 排序最靠前的 camera 才会生效
- ui 方案推荐使用 unity ugui
- 使用时需要先引入 core 代码库， https://git.sofunny.io/engine/packages/com.unity.render-pipelines.core.mini
- render feature: simple outline feature，简单挤出式描边效果

### ChangeLog
```
#0928
issue: 删除 mini rp 关于实时 addtionallight 的功能，包括 shader compile，但是不能影响烘焙
- 修改后 additionallight 可以不占用分配
- urp 原生 shader 如果使用了 keyword ，将不渲染，所以将所有 shader 的 keyword 删掉 _ADDITIONAL_LIGHT_SHADOWS
- Done

issue: 删除关于 empty shadow 分配和 configure empty shadow target 的功能
- 没有删除分配，但是 configure empty shadow target 似乎没有了
- 现在如果从摄影机关闭 shadow，main shadow target 会遗留下来，必须管线里关闭
- 现在如果在 light 上关闭 shadow，main shadow target 也会遗留下来，必须管线里关闭

issue: 修复 js 关于 no valid shadow casters 的报错
- 在关闭实时 additional light map 之后，已经没有这个报错了
- Android 真机上依旧有报错

issue: 让 minilit shader 支持 additionallight 烘焙
- 未完成，但是如果想用点光源烘焙的结果，可以使用 urp 的 simple lit

issue: 让 mini rp 能够强制仅使用但相机方案
- 在 game 相机数量大于 1 时，只保留第一个排序后的第一个
- 关闭 camera stack 功能
- 在 overlay camera 被移除后，原本 camera 会报错
- 禁用了 camera stack 的添加功能
- Done

issue: 通过 WX_PERFORMANCE_MODE 剔除部分 XR 代码与 shader 变体
- Done

#0929
issues: 通过 camera data 来关闭 post process
- 未完成，目前 post process 是通过关闭 post processing data 来关闭的

issues: mini rp 的 minilit 还是需要支持实时点光源功能
- 交给了 YN 开发中
- 已提交合并

#1009
issues: 添加简单 outline feature
- 使用简单挤出的方法，保证 srp batch
- Done

issues: additional lights cookies format 警告
- !Additional Lights Cookie Format(GrayscaleHigh)is not supported by the platform.Falling back to 32 bit format(RGBA8 UNorm)

issues: mini lit 数据转换 警告
- GLSL link error:ERROR:0:10:'assign': cannot convert from 'float' to '4-component vector of float' ERROR:1 compilation errors. No code generated.

issues: ShadowAuto 警告
- 'ShadowAuto'is not supported. RenderTexture::GetTemporary fallbacks to DepthAuto format on this platform.Use 'SystemInfo IsFormatSupported'C#API to check format support.

#1010
issues: 关闭 additonal light cookie 以及 additional light shadow map
- Done

#1011
issues: 对 renderer data 的设置导致 rt 数量翻倍的 bug
- 检查 color attachement 也同样会在 bug 出现时翻倍

issues: simple outline feature 会产生 gc
- 检查材质资源创建
- 检查 profiler tag 字符
- Done

issues: 添加 internal scope 之外控制渲染管线的接口 MiniRPController
- 通过静态类 MiniRPController 来调用
- Done
```


## Getting started

To make it easy for you to get started with GitLab, here's a list of recommended next steps.

Already a pro? Just edit this README.md and make it your own. Want to make it easy? [Use the template at the bottom](#editing-this-readme)!

## Add your files

- [ ] [Create](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#create-a-file) or [upload](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#upload-a-file) files
- [ ] [Add files using the command line](https://docs.gitlab.com/ee/gitlab-basics/add-file.html#add-a-file-using-the-command-line) or push an existing Git repository with the following command:

```
cd existing_repo
git remote add origin https://git.sofunny.io/engine/packages/com.unity.render-pipelines.universal.mini.git
git branch -M main
git push -uf origin main
```

## Integrate with your tools

- [ ] [Set up project integrations](https://git.sofunny.io/engine/packages/com.unity.render-pipelines.universal.mini/-/settings/integrations)

## Collaborate with your team

- [ ] [Invite team members and collaborators](https://docs.gitlab.com/ee/user/project/members/)
- [ ] [Create a new merge request](https://docs.gitlab.com/ee/user/project/merge_requests/creating_merge_requests.html)
- [ ] [Automatically close issues from merge requests](https://docs.gitlab.com/ee/user/project/issues/managing_issues.html#closing-issues-automatically)
- [ ] [Enable merge request approvals](https://docs.gitlab.com/ee/user/project/merge_requests/approvals/)
- [ ] [Automatically merge when pipeline succeeds](https://docs.gitlab.com/ee/user/project/merge_requests/merge_when_pipeline_succeeds.html)

## Test and Deploy

Use the built-in continuous integration in GitLab.

- [ ] [Get started with GitLab CI/CD](https://docs.gitlab.com/ee/ci/quick_start/index.html)
- [ ] [Analyze your code for known vulnerabilities with Static Application Security Testing(SAST)](https://docs.gitlab.com/ee/user/application_security/sast/)
- [ ] [Deploy to Kubernetes, Amazon EC2, or Amazon ECS using Auto Deploy](https://docs.gitlab.com/ee/topics/autodevops/requirements.html)
- [ ] [Use pull-based deployments for improved Kubernetes management](https://docs.gitlab.com/ee/user/clusters/agent/)
- [ ] [Set up protected environments](https://docs.gitlab.com/ee/ci/environments/protected_environments.html)

***

# Editing this README

When you're ready to make this README your own, just edit this file and use the handy template below (or feel free to structure it however you want - this is just a starting point!). Thank you to [makeareadme.com](https://www.makeareadme.com/) for this template.

## Suggestions for a good README
Every project is different, so consider which of these sections apply to yours. The sections used in the template are suggestions for most open source projects. Also keep in mind that while a README can be too long and detailed, too long is better than too short. If you think your README is too long, consider utilizing another form of documentation rather than cutting out information.

## Name
Choose a self-explaining name for your project.

## Description
Let people know what your project can do specifically. Provide context and add a link to any reference visitors might be unfamiliar with. A list of Features or a Background subsection can also be added here. If there are alternatives to your project, this is a good place to list differentiating factors.

## Badges
On some READMEs, you may see small images that convey metadata, such as whether or not all the tests are passing for the project. You can use Shields to add some to your README. Many services also have instructions for adding a badge.

## Visuals
Depending on what you are making, it can be a good idea to include screenshots or even a video (you'll frequently see GIFs rather than actual videos). Tools like ttygif can help, but check out Asciinema for a more sophisticated method.

## Installation
Within a particular ecosystem, there may be a common way of installing things, such as using Yarn, NuGet, or Homebrew. However, consider the possibility that whoever is reading your README is a novice and would like more guidance. Listing specific steps helps remove ambiguity and gets people to using your project as quickly as possible. If it only runs in a specific context like a particular programming language version or operating system or has dependencies that have to be installed manually, also add a Requirements subsection.

## Usage
Use examples liberally, and show the expected output if you can. It's helpful to have inline the smallest example of usage that you can demonstrate, while providing links to more sophisticated examples if they are too long to reasonably include in the README.

## Support
Tell people where they can go to for help. It can be any combination of an issue tracker, a chat room, an email address, etc.

## Roadmap
If you have ideas for releases in the future, it is a good idea to list them in the README.

## Contributing
State if you are open to contributions and what your requirements are for accepting them.

For people who want to make changes to your project, it's helpful to have some documentation on how to get started. Perhaps there is a script that they should run or some environment variables that they need to set. Make these steps explicit. These instructions could also be useful to your future self.

You can also document commands to lint the code or run tests. These steps help to ensure high code quality and reduce the likelihood that the changes inadvertently break something. Having instructions for running tests is especially helpful if it requires external setup, such as starting a Selenium server for testing in a browser.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.

## Project status
If you have run out of energy or time for your project, put a note at the top of the README saying that development has slowed down or stopped completely. Someone may choose to fork your project or volunteer to step in as a maintainer or owner, allowing your project to keep going. You can also make an explicit request for maintainers.
