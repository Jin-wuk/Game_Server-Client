# 데이터베이스 스키마 설계

(예정)

## 1. 사용자 및 자산 (Users)
| 컬럼명 | 타입 | 설명 |
| :--- | :--- | :--- |
| `id` | BIGINT (PK) | 고유 사용자 ID |
| `username` | VARCHAR | 계정명 |
| `level` | INT | 캐릭터 레벨 |
| `exp` | BIGINT | 현재 경험치 |
| `gold` | BIGINT | 보유 아데나 |

## 2. 인벤토리 및 아이템 (Inventory)
| 컬럼명 | 타입 | 설명 |
| :--- | :--- | :--- |
| `item_instance_id` | BIGINT (PK) | 개별 아이템 고유 번호 |
| `owner_id` | BIGINT (FK) | 소유자 ID |
| `item_id` | INT | 아이템 마스터 코드 |
| `enhance_level` | INT | 강화 수치 (+0~+9) |
| `is_tradeable` | BOOL | 거래 가능 여부 (비각인 여부) |

## 3. 거래소 (Exchange Market)
| 컬럼명 | 타입 | 설명 |
| :--- | :--- | :--- |
| `trade_id` | BIGINT (PK) | 거래 번호 |
| `seller_id` | BIGINT | 판매자 ID |
| `item_data_json` | TEXT | 판매 아이템 정보 (강화도 포함) |
| `price` | BIGINT | 판매 희망 가격 |
| `status` | ENUM | 판매중 / 판매완료 / 취소 |



## 4. 로그 테이블 (Logs)
- **Gold_Log**: 골드 입출력 내역 (사유 포함)
- **Enhance_Log**: 강화 시도 및 결과 (성공/증발)