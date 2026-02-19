# 프로젝트
> **서버 권위적 수치 검증 시스템을 갖춘 방치형 2D RPG**

## 1. 프로젝트 개요
- **플레이 방식**: 싱글 플레이 중심의 전투 및 성장
- **멀티 요소**: 서버 기반 거래소, 통합 랭킹 시스템

## 2. 기술 스택
- **Client**: Unity 2022.3 LTS (C#)
- **Server**: ASP.NET Core Web API (.NET 8)
- **Database**: MySQL (Entity Framework Core)
- **Communication**: JSON over HTTP (REST API)

## 3. 폴더 구조 (Monorepo)
- `/Client`: 유니티 프로젝트 (플레이 로직 및 UI)
- `/Server`: ASP.NET Core 프로젝트 (데이터 검증 및 DB 관리)

## 4. 주요 기능
- **서버 권위적 경제**: 모든 골드 획득 및 아이템 소모는 서버 검증 후 반영
- **거래소**: 유저 간 아이템 매매 (트랜잭션 보장)
- **강화 시스템**: 서버에서 확률을 계산하여 아이템 등급 결정
...